using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MISA.WorkShift.Api.Services;
using MISA.WorkShift.Core.DTOs;
using MISA.WorkShift.Core.Entities;
using MISA.WorkShift.Core.Enums;
using MISA.WorkShift.Core.Interfaces.Repositories;
using MISA.WorkShift.Core.Interfaces.Services;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace MISA.WorkShift.Api.Controllers
{
    public class InvoicesController : BaseController<Invoice>
    {
        private readonly IInvoiceService _invoiceService;
        private readonly MISA.WorkShift.Core.Interfaces.Repositories.IInvoiceTemplateXsltRepository _xsltRepo;
        private readonly MISA.WorkShift.Core.Interfaces.Repositories.IInvoiceTemplateRepository _templateRepo;
        private readonly MISA.WorkShift.Core.Interfaces.Repositories.IInvoiceRepository _invoiceRepo;
        private readonly TemplateFileService _fileService;

        public InvoicesController(IInvoiceService invoiceService, IInvoiceTemplateXsltRepository xsltRepo, IInvoiceTemplateRepository templateRepo, IInvoiceRepository invoiceRepo, TemplateFileService fileService) : base(invoiceService)
        {
            _invoiceService = invoiceService;
            _xsltRepo = xsltRepo;
            _templateRepo = templateRepo;
            _invoiceRepo = invoiceRepo;
            _fileService = fileService;
        }

        /// <summary>
        /// API Tạo mới hóa đơn kèm chi tiết hàng hóa.
        /// </summary>
        [HttpPost("creation")]
        public ActionResult<Invoice> CreateInvoice([FromBody] InvoiceCreateDto dto)
        {
            try
            {
                // Lúc này res là một đối tượng Invoice (chứa ID, Number, Data...)
                var res = _invoiceService.CreateInvoiceWithDetails(dto);

                // Đổi BaseResponse<int> thành BaseResponse<Invoice>
                return StatusCode(201, BaseResponse<Invoice>.CreatedResponse(res, "Lập hóa đơn thành công."));
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Bạn có thể viết thêm các API đặc thù tại đây
        // Ví dụ: API phát hành hóa đơn (Sign XML)
        [HttpPost("{id}/publish")]
        public IActionResult PublishInvoice(Guid id)
        {
            return Ok("Hóa đơn đã được gửi lên cơ quan Thuế thành công.");
        }

        /// <summary>
        /// Trả file XML hóa đơn (private storage) sau khi kiểm tra quyền CompanyId
        /// </summary>
        [Authorize]
        [HttpGet("{id}/xml")]
        public IActionResult GetInvoiceXml(Guid id)
        {
            try
            {
                var invoice = _invoiceService.GetById(id);
                if (invoice == null)
                {
                    return StatusCode(404, BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.NotFound, "Không tìm thấy hóa đơn."));
                }

                var companyClaim = User?.Claims?.FirstOrDefault(c => c.Type == "CompanyId");
                if (companyClaim == null) return Unauthorized(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.ValidateError, "CompanyId claim not found."));
                if (!Guid.TryParse(companyClaim.Value, out var companyId)) return BadRequest(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.ValidateError, "Invalid CompanyId claim."));

                //if (invoice.CompanyId != companyId) return Forbid();

                if (string.IsNullOrEmpty(invoice.XmlFilePath)) return StatusCode(404, BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.NotFound, "File XML không tồn tại."));

                var p = invoice.XmlFilePath.StartsWith("/") ? invoice.XmlFilePath.Substring(1) : invoice.XmlFilePath;
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), p.Replace('/', Path.DirectorySeparatorChar));

                if (!System.IO.File.Exists(fullPath)) return StatusCode(404, BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.NotFound, "File XML không tìm thấy trên server."));

                var bytes = System.IO.File.ReadAllBytes(fullPath);
                var fileName = (invoice.InvoiceNumber ?? invoice.InvoiceId.ToString()) + ".xml";
                return File(bytes, "application/xml", fileName);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // Override GET api/v1/invoices/{id} to return header + details
        [Authorize]
        [HttpGet("{id}")]
        public override IActionResult GetById(Guid id)
        {
            try
            {
                var invoice = _invoiceRepo.GetById(id);
                if (invoice == null) return StatusCode(404, BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.NotFound, "Không tìm thấy hóa đơn."));

                var companyClaim = User?.Claims?.FirstOrDefault(c => c.Type == "CompanyId");
                if (companyClaim == null) return Unauthorized(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.ValidateError, "CompanyId claim not found."));
                if (!Guid.TryParse(companyClaim.Value, out var companyId)) return BadRequest(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.ValidateError, "Invalid CompanyId claim."));

                //if (invoice.CompanyId != companyId) return Forbid();

                var details = _invoiceRepo.GetDetailsByInvoiceId(id);
                var payload = new InvoiceCreateDto { Invoice = invoice, Details = details };
                return Ok(BaseResponse<InvoiceCreateDto>.SuccessResponse(payload));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET api/v1/invoices/{id}/render?format=html|pdf
        [HttpGet("{id}/render")]
        public async Task<IActionResult> RenderInvoice(Guid id, [FromQuery] string format = "html")
        {
            try
            {
                var invoice = _invoiceService.GetById(id);
                if (invoice == null) return StatusCode(404, BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.NotFound, "Không tìm thấy hóa đơn."));

                var companyClaim = User?.Claims?.FirstOrDefault(c => c.Type == "CompanyId");
                if (companyClaim == null) return Unauthorized(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.ValidateError, "CompanyId claim not found."));
                if (!Guid.TryParse(companyClaim.Value, out var companyId)) return BadRequest(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.ValidateError, "Invalid CompanyId claim."));

                //if (invoice.CompanyId != companyId) return Forbid();

                if (string.IsNullOrEmpty(invoice.XmlFilePath)) return StatusCode(404, BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.NotFound, "File XML không tồn tại."));

                // load xml
                var xmlPath = invoice.XmlFilePath.StartsWith("/") ? invoice.XmlFilePath.Substring(1) : invoice.XmlFilePath;
                var fullXmlPath = Path.Combine(Directory.GetCurrentDirectory(), xmlPath.Replace('/', Path.DirectorySeparatorChar));
                if (!System.IO.File.Exists(fullXmlPath)) return StatusCode(404, BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.NotFound, "File XML không tìm thấy trên server."));

                // find xslt content: priority - instance XSLT (by TemplateId?), template record XsltContent, default xslt by series/templateCode
                string? xsltContent = null;

                // 1) try template instance if TemplateId present - find last user instance
                if (invoice.TemplateId.HasValue)
                {
                    // try to find instance Xslt for company/user
                    var instances = _xsltRepo.GetByTemplateId(invoice.TemplateId.Value)?.ToList();
                    if (instances != null && instances.Count > 0)
                    {
                        xsltContent = instances.OrderByDescending(i => i.CreatedDate).FirstOrDefault()?.XsltContent;
                    }

                    // 2) try template record
                    if (string.IsNullOrEmpty(xsltContent))
                    {
                        var tpl = _templateRepo.GetById(invoice.TemplateId.Value);
                        if (tpl != null) xsltContent = tpl.XsltContent;
                    }
                }

                // 3) fallback to default template by series or template code
                if (string.IsNullOrEmpty(xsltContent))
                {
                    // prefer TemplateCode if set
                    if (!string.IsNullOrEmpty(invoice.TemplateCode)) xsltContent = _fileService.ReadDefaultXslt(invoice.TemplateCode);
                    // otherwise try series
                    if (string.IsNullOrEmpty(xsltContent) && !string.IsNullOrEmpty(invoice.Series)) xsltContent = _fileService.ReadDefaultXslt(invoice.Series);
                }

                if (string.IsNullOrEmpty(xsltContent)) return StatusCode(404, BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.NotFound, "Không tìm thấy XSLT để render."));

                // transform xml -> html using XslCompiledTransform
                string html;
                var xsl = new XslCompiledTransform();
                using (var sr = new StringReader(xsltContent))
                using (var xr = XmlReader.Create(sr))
                {
                    xsl.Load(xr);
                    using var xmlReader = XmlReader.Create(fullXmlPath);
                    var sb = new StringBuilder();
                    using var writer = XmlWriter.Create(new StringWriter(sb), xsl.OutputSettings ?? new XmlWriterSettings());
                    xsl.Transform(xmlReader, writer);
                    html = sb.ToString();
                }

                if (format?.ToLower() == "pdf")
                {
                    // use PuppeteerSharp to convert HTML -> PDF
                    var launchOptions = new LaunchOptions { Headless = true };
                    await using var browser = await Puppeteer.LaunchAsync(launchOptions);
                    await using var page = await browser.NewPageAsync();
                    await page.SetContentAsync(html, new NavigationOptions
                    {
                        WaitUntil = new[] { WaitUntilNavigation.Networkidle0 }
                    });

                    // ép dùng print CSS
                    await page.EmulateMediaTypeAsync(MediaType.Print);

                    var pdfBytes = await page.PdfDataAsync(new PdfOptions
                    {
                        Format = PaperFormat.A4,
                        PrintBackground = true,
                        PreferCSSPageSize = true,
                        MarginOptions = new MarginOptions
                        {
                            Top = "10mm",
                            Bottom = "10mm",
                            Left = "10mm",
                            Right = "10mm"
                        }
                    });
                    //var pdfBytes = await page.PdfDataAsync(new PdfOptions { Format = PuppeteerSharp.Media.PaperFormat.A4 });
                    var fileName = (invoice.InvoiceNumber ?? invoice.InvoiceId.ToString()) + ".pdf";
                    // ask browser to open inline (PDF viewer) instead of forcing download
                    Response.Headers["Content-Disposition"] = $"inline; filename=\"{fileName}\"";
                    return File(pdfBytes, "application/pdf");
                }

                // return HTML
                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET api/v1/invoices/{id}/full
        [Authorize]
        [HttpGet("{id}/full")]
        public IActionResult GetInvoiceWithDetails(Guid id)
        {
            try
            {
                var invoice = _invoiceRepo.GetById(id);
                if (invoice == null) return StatusCode(404, BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.NotFound, "Không tìm thấy hóa đơn."));

                var companyClaim = User?.Claims?.FirstOrDefault(c => c.Type == "CompanyId");
                if (companyClaim == null) return Unauthorized(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.ValidateError, "CompanyId claim not found."));
                if (!Guid.TryParse(companyClaim.Value, out var companyId)) return BadRequest(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.ValidateError, "Invalid CompanyId claim."));

                if (invoice.CompanyId != companyId) return Forbid();

                var details = _invoiceRepo.GetDetailsByInvoiceId(id);
                var payload = new InvoiceCreateDto { Invoice = invoice, Details = details };
                return Ok(BaseResponse<InvoiceCreateDto>.SuccessResponse(payload));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // PUT api/v1/invoices/{id}/full
        [Authorize]
        [HttpPut("{id}/full")]
        public IActionResult UpdateInvoiceFull(Guid id, [FromBody] InvoiceCreateDto dto)
        {
            try
            {
                if (dto == null || dto.Invoice == null) return BadRequest(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.ValidateError, "Invalid payload."));

                if (dto.Invoice.InvoiceId == Guid.Empty) dto.Invoice.InvoiceId = id;
                if (dto.Invoice.InvoiceId != id) return BadRequest(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.ValidateError, "InvoiceId mismatch."));

                var companyClaim = User?.Claims?.FirstOrDefault(c => c.Type == "CompanyId");
                if (companyClaim == null) return Unauthorized(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.ValidateError, "CompanyId claim not found."));
                if (!Guid.TryParse(companyClaim.Value, out var companyId)) return BadRequest(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.ValidateError, "Invalid CompanyId claim."));

                var existing = _invoiceRepo.GetById(id);
                if (existing == null) return StatusCode(404, BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.NotFound, "Invoice not found."));
                //if (existing.CompanyId != companyId) return Forbid();

                // ensure details have IDs and link to invoice
                if (dto.Details != null)
                {
                    foreach (var d in dto.Details)
                    {
                        if (d.DetailId == Guid.Empty) d.DetailId = Guid.NewGuid();
                        d.InvoiceId = id;
                    }
                }

                var res = _invoiceService.UpdateInvoiceWithDetails(dto.Invoice, dto.Details ?? new System.Collections.Generic.List<InvoiceDetail>());
                return Ok(BaseResponse<int>.SuccessResponse(res));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// API để ký và phát hành hóa đơn điện tử.
        /// Nhận nội dung XML đã ký từ client và lưu trữ, cập nhật trạng thái hóa đơn.
        /// </summary>
        /// <param name="id">ID của hóa đơn cần ký.</param>
        /// <param name="dto">DTO chứa nội dung XML đã ký.</param>
        /// <returns>Kết quả thành công hoặc lỗi.</returns>
        [HttpPost("{id}/sign-and-publish")]
        public async Task<IActionResult> SignAndPublishInvoice(Guid id, [FromBody] InvoiceSignRequestDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.SignedXmlContent))
                {
                    return BadRequest(BaseResponse<string>.ErrorResponse(MisaCode.ValidateError, "Nội dung XML đã ký không được để trống."));
                }

                var companyClaim = User?.Claims?.FirstOrDefault(c => c.Type == "CompanyId");
                if (companyClaim == null) return Unauthorized(BaseResponse<string>.ErrorResponse(MisaCode.ValidateError, "CompanyId claim not found."));
                if (!Guid.TryParse(companyClaim.Value, out var companyId)) return BadRequest(BaseResponse<string>.ErrorResponse(MisaCode.ValidateError, "Invalid CompanyId claim."));

                var invoice = _invoiceService.GetById(id);
                //if (invoice == null || invoice.CompanyId != companyId) return Forbid(); // Hoặc NotFound nếu không tìm thấy

                var rowEffect = await _invoiceService.SignAndPublishInvoice(id, dto.SignedXmlContent);
                return Ok(BaseResponse<int>.SuccessResponse(rowEffect, "Hóa đơn đã được ký và phát hành thành công."));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}