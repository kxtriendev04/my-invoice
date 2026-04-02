using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MISA.WorkShift.Core.Interfaces.Repositories;
using MISA.WorkShift.Core.Entities;
using MISA.WorkShift.Core.DTOs;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using MISA.WorkShift.Api.Services;

namespace MISA.WorkShift.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class InvoiceTemplatesController : ControllerBase
    {
        private readonly IInvoiceTemplateRepository _templateRepo;
        private readonly IInvoiceTemplateXsltRepository _xsltRepo;
        private readonly MISA.WorkShift.Api.Services.TemplateFileService _fileService;

        public InvoiceTemplatesController(IInvoiceTemplateRepository templateRepo, IInvoiceTemplateXsltRepository xsltRepo, MISA.WorkShift.Api.Services.TemplateFileService fileService)
        {
            _templateRepo = templateRepo;
            _xsltRepo = xsltRepo;
            _fileService = fileService;
        }

        // GET api/v1/invoicetemplates/defaults
        // Optional filters: category, invoiceType, size, taxRate, search (displayName or templateCode)
        [HttpGet("defaults")]
        public IActionResult GetDefaults([FromQuery] MISA.WorkShift.Core.Enums.TemplateCategoryEnum? category = null,
            [FromQuery] MISA.WorkShift.Core.Enums.TemplateInvoiceTypeEnum? invoiceType = null,
            [FromQuery] MISA.WorkShift.Core.Enums.PaperSizeEnum? size = null,
            [FromQuery] string? taxRate = null, [FromQuery] string? search = null)
        {
            var list = _fileService.GetDefaultTemplates().ToList();

            if (category.HasValue)
            {
                var catName = category.Value.ToString().ToLower();
                list = list.Where(l =>
                    string.Equals(l.Category, catName, StringComparison.OrdinalIgnoreCase)
                    || (!string.IsNullOrEmpty(l.TemplateCode) && l.TemplateCode.Contains(catName, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            if (invoiceType.HasValue)
            {
                var invName = invoiceType.Value.ToString();
                list = list.Where(l =>
                    string.Equals(l.InvoiceType, invName, StringComparison.OrdinalIgnoreCase)
                    || (!string.IsNullOrEmpty(l.TemplateCode) && l.TemplateCode.Contains(invName, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            if (size.HasValue)
            {
                var sizeName = size.Value.ToString();
                list = list.Where(l => l.Sizes != null && l.Sizes.Any(s => string.Equals(s, sizeName, StringComparison.OrdinalIgnoreCase))).ToList();
            }

            if (!string.IsNullOrEmpty(taxRate))
                list = list.Where(l => l.TaxRates != null && l.TaxRates.Any(t => string.Equals(t, taxRate, StringComparison.OrdinalIgnoreCase))).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(l => (l.DisplayName != null && l.DisplayName.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                                       (l.TemplateCode != null && l.TemplateCode.Contains(search, StringComparison.OrdinalIgnoreCase))).ToList();
            }

            return Ok(BaseResponse<IEnumerable<DefaultTemplateInfo>>.SuccessResponse(list));
        }

        // GET api/v1/invoicetemplates/defaults/{templateCode}/payload
        [HttpGet("defaults/{templateCode}/payload")]
        public IActionResult GetDefaultPayload(string templateCode)
        {
            var meta = _fileService.GetDefaultTemplates().FirstOrDefault(m => string.Equals(m.TemplateCode, templateCode, StringComparison.OrdinalIgnoreCase));
            if (meta == null) return NotFound(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.NotFound, "Template not found."));

            var xslt = _fileService.ReadDefaultXslt(templateCode) ?? string.Empty;
            var previewBase64 = string.IsNullOrEmpty(meta.PreviewRelativePath) ? null : _fileService.ReadFileAsBase64(meta.PreviewRelativePath);

            var payload = new
            {
                meta.TemplateCode,
                meta.DisplayName,
                meta.PreviewRelativePath,
                PreviewBase64 = previewBase64,
                XsltContent = xslt,
                Meta = meta
            };

            return Ok(BaseResponse<object>.SuccessResponse(payload));
        }

        public class InitByCodeDto { public string TemplateCode { get; set; } }

        public class CreateTemplateDto
        {
            public string? Background { get; set; }
            public string InvSeries { get; set; }
            public int InvoiceType { get; set; }
            public string? Logo { get; set; }
            public string TemplateCode { get; set; }
            public string TemplateName { get; set; }
        }

        public class UpdateTemplateDto
        {
            public string? TemplateName { get; set; }
            public string? Background { get; set; }
            public string? Logo { get; set; }
            public int? InvoiceType { get; set; }
            public string? XsltContent { get; set; }
        }

        // POST api/v1/invoicetemplates/init-by-code
        [HttpPost("init-by-code")]
        public IActionResult InitByCode([FromBody] InitByCodeDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.TemplateCode)) return BadRequest(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.ValidateError, "templateCode is required."));

            var userClaim = User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            var companyClaim = User.Claims.FirstOrDefault(x => x.Type == "CompanyId");
            if (userClaim == null || companyClaim == null) return Unauthorized();

            if (!Guid.TryParse(userClaim.Value, out var userId)) return Unauthorized();
            if (!Guid.TryParse(companyClaim.Value, out var companyId)) return Unauthorized();

            var meta = _fileService.GetDefaultTemplates().FirstOrDefault(m => string.Equals(m.TemplateCode, dto.TemplateCode, StringComparison.OrdinalIgnoreCase));
            if (meta == null) return NotFound(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.NotFound, "Template not found."));

            var xslt = _fileService.ReadDefaultXslt(dto.TemplateCode) ?? string.Empty;

            var relativePath = _fileService.SaveXsltToFile(companyId, userId, xslt, fileName: dto.TemplateCode + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xslt");

            var instance = new InvoiceTemplateXslt
            {
                InstanceId = Guid.NewGuid(),
                TemplateId = null,
                CompanyId = companyId,
                UserId = userId,
                XsltContent = xslt,
                CreatedDate = DateTime.Now,
                CreatedBy = User.Identity?.Name
            };
            _xsltRepo.Insert(instance);

            return Ok(BaseResponse<object>.SuccessResponse(new { instanceId = instance.InstanceId, path = relativePath }));
        }

        // POST api/v1/invoicetemplates/create
        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateTemplateDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.TemplateCode) || string.IsNullOrEmpty(dto.TemplateName))
                return BadRequest(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.ValidateError, "templateCode and templateName are required."));

            var userClaim = User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            var companyClaim = User.Claims.FirstOrDefault(x => x.Type == "CompanyId");
            if (userClaim == null || companyClaim == null) return Unauthorized();

            if (!Guid.TryParse(userClaim.Value, out var userId)) return Unauthorized();
            if (!Guid.TryParse(companyClaim.Value, out var companyId)) return Unauthorized();

            var template = new InvoiceTemplate
            {
                TemplateId = Guid.NewGuid(),
                CompanyId = companyId,
                TemplateCode = dto.TemplateCode,
                TemplateName = dto.TemplateName,
                InvSeries = dto.InvSeries,
                CssCustom = JsonSerializer.Serialize(new { background = dto.Background, logo = dto.Logo, invoiceType = dto.InvoiceType }),
                XsltContent = null,
                IsDefault = false,
                CreatedDate = DateTime.Now,
                CreatedBy = User.Identity?.Name
            };

            _templateRepo.Insert(template);

            return Ok(BaseResponse<object>.SuccessResponse(new { templateId = template.TemplateId }));
        }

        // PATCH api/v1/invoicetemplates/{templateId}
        // GET api/v1/invoicetemplates/{templateId}
        [HttpGet("{templateId}")]
        public IActionResult GetById(Guid templateId)
        {
            var userClaim = User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            var companyClaim = User.Claims.FirstOrDefault(x => x.Type == "CompanyId");
            if (userClaim == null || companyClaim == null) return Unauthorized();

            if (!Guid.TryParse(userClaim.Value, out var userId)) return Unauthorized();
            if (!Guid.TryParse(companyClaim.Value, out var companyId)) return Unauthorized();

            var template = _templateRepo.GetById(templateId);
            if (template == null) return NotFound(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.NotFound, "Template not found."));
            if (template.CompanyId != companyId) return Forbid();

            // try get default meta if exists (for default templates)
            var defaultMeta = _fileService.GetDefaultTemplates().FirstOrDefault(m => string.Equals(m.TemplateCode, template.TemplateCode, StringComparison.OrdinalIgnoreCase));

            // xslt content: prefer default resource, otherwise stored content
            var xslt = _fileService.ReadDefaultXslt(template.TemplateCode);
            var xsltContent = !string.IsNullOrEmpty(xslt) ? xslt : (template.XsltContent ?? string.Empty);

            string? previewBase64 = null;
            if (defaultMeta != null && !string.IsNullOrEmpty(defaultMeta.PreviewRelativePath))
                previewBase64 = _fileService.ReadFileAsBase64(defaultMeta.PreviewRelativePath);

            object metaObj = defaultMeta != null
                ? (object)defaultMeta
                : new
                {
                    templateCode = template.TemplateCode,
                    fileName = string.Empty,
                    relativePath = (string?)null,
                    displayName = template.TemplateName,
                    previewRelativePath = (string?)null,
                    category = "custom",
                    invoiceType = template.InvSeries,
                    sizes = Array.Empty<string>(),
                    taxRates = Array.Empty<string>()
                };

            var payload = new
            {
                templateCode = template.TemplateCode,
                displayName = defaultMeta?.DisplayName ?? template.TemplateName,
                previewRelativePath = defaultMeta?.PreviewRelativePath,
                previewBase64 = previewBase64,
                xsltContent = xsltContent,
                meta = metaObj
            };

            return Ok(BaseResponse<object>.SuccessResponse(payload, "Thành công."));
        }

        [HttpPatch("{templateId}")]
        public IActionResult UpdateTemplate(Guid templateId, [FromBody] UpdateTemplateDto dto)
        {
            if (dto == null) return BadRequest(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.ValidateError, "No data provided."));

            var userClaim = User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            var companyClaim = User.Claims.FirstOrDefault(x => x.Type == "CompanyId");
            if (userClaim == null || companyClaim == null) return Unauthorized();

            if (!Guid.TryParse(userClaim.Value, out var userId)) return Unauthorized();
            if (!Guid.TryParse(companyClaim.Value, out var companyId)) return Unauthorized();

            var template = _templateRepo.GetById(templateId);
            if (template == null) return NotFound(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.NotFound, "Template not found."));
            if (template.CompanyId != companyId) return Forbid();

            // update allowed fields
            if (!string.IsNullOrEmpty(dto.TemplateName)) template.TemplateName = dto.TemplateName;

            // merge CssCustom json
            try
            {
                var existing = string.IsNullOrEmpty(template.CssCustom) ? new Dictionary<string, object?>() : JsonSerializer.Deserialize<Dictionary<string, object?>>(template.CssCustom) ?? new Dictionary<string, object?>();

                if (dto.Background != null) existing["background"] = dto.Background;
                if (dto.Logo != null) existing["logo"] = dto.Logo;
                if (dto.InvoiceType.HasValue) existing["invoiceType"] = dto.InvoiceType.Value;

                template.CssCustom = JsonSerializer.Serialize(existing);
            }
            catch
            {
                // fallback: overwrite with minimal css custom
                template.CssCustom = JsonSerializer.Serialize(new { background = dto.Background, logo = dto.Logo, invoiceType = dto.InvoiceType });
            }

            if (dto.XsltContent != null)
            {
                template.XsltContent = dto.XsltContent;
                // optionally save file via service
                // _fileService.SaveXsltToFile(companyId, userId, dto.XsltContent);
            }

            template.ModifiedDate = DateTime.Now;
            template.ModifiedBy = User.Identity?.Name;

            _templateRepo.Update(templateId, template);

            return Ok(BaseResponse<object>.SuccessResponse(new { templateId = template.TemplateId }));
        }

        // GET api/v1/invoicetemplates/by-company?companyId={id}
        [HttpGet("by-company")]
        public IActionResult GetByCompany([FromQuery] Guid? companyId = null)
        {
            if (companyId == null)
            {
                var c = User.Claims.FirstOrDefault(x => x.Type == "CompanyId");
                if (c == null) return Unauthorized(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.ValidateError, "CompanyId claim not found."));
                if (!Guid.TryParse(c.Value, out var parsed)) return BadRequest(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.ValidateError, "Invalid CompanyId claim."));
                companyId = parsed;
            }

            var data = _templateRepo.GetByCompanyId(companyId.Value);
            return Ok(BaseResponse<IEnumerable<InvoiceTemplate>>.SuccessResponse(data));
        }

        // POST api/v1/invoicetemplates/{templateId}/init-for-user
        [HttpPost("{templateId}/init-for-user")]
        public IActionResult InitForUser(Guid templateId)
        {
            var userClaim = User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            var companyClaim = User.Claims.FirstOrDefault(x => x.Type == "CompanyId");
            if (userClaim == null || companyClaim == null) return Unauthorized();

            if (!Guid.TryParse(userClaim.Value, out var userId)) return Unauthorized();
            if (!Guid.TryParse(companyClaim.Value, out var companyId)) return Unauthorized();

            var template = _templateRepo.GetById(templateId);
            if (template == null) return NotFound(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.NotFound, "Template not found."));

            // read default xslt resource by template code first
            var defaultXslt = _fileService.ReadDefaultXslt(template.TemplateCode);
            var xsltContent = !string.IsNullOrEmpty(defaultXslt) ? defaultXslt : (template.XsltContent ?? string.Empty);

            // optionally save file copy for user and store instance record
            var relativePath = _fileService.SaveXsltToFile(companyId, userId, xsltContent);

            var instance = new InvoiceTemplateXslt
            {
                InstanceId = Guid.NewGuid(),
                TemplateId = templateId,
                CompanyId = companyId,
                UserId = userId,
                XsltContent = xsltContent,
                CreatedDate = DateTime.Now,
                CreatedBy = User.Identity?.Name
            };
            _xsltRepo.Insert(instance);

            return Ok(BaseResponse<object>.SuccessResponse(new { instanceId = instance.InstanceId, path = relativePath }));
        }

        // GET api/v1/invoicetemplates/instances/my
        [HttpGet("instances/my")]
        public IActionResult GetMyInstances()
        {
            var userClaim = User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userClaim == null) return Unauthorized();
            if (!Guid.TryParse(userClaim.Value, out var userId)) return Unauthorized();
            var data = _xsltRepo.GetByUserId(userId);
            return Ok(BaseResponse<IEnumerable<InvoiceTemplateXslt>>.SuccessResponse(data));
        }

        // DELETE api/v1/invoicetemplates/{templateId}
        [HttpDelete("{templateId}")]
        public IActionResult DeleteTemplate(Guid templateId)
        {
            var userClaim = User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            var companyClaim = User.Claims.FirstOrDefault(x => x.Type == "CompanyId");
            if (userClaim == null || companyClaim == null) return Unauthorized();

            if (!Guid.TryParse(companyClaim.Value, out var companyId)) return Unauthorized();

            var template = _templateRepo.GetById(templateId);
            if (template == null) return NotFound(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.NotFound, "Template not found."));
            if (template.CompanyId != companyId) return Forbid();

            // n?u có t? khai (InvoiceRegistration) ?ã ???c g?i lên CQT thì không cho xoá (ch? ???c xem)
            var regRepo = HttpContext.RequestServices.GetService(typeof(MISA.WorkShift.Core.Interfaces.Repositories.IInvoiceRegistrationRepository)) as MISA.WorkShift.Core.Interfaces.Repositories.IInvoiceRegistrationRepository;
            if (regRepo != null)
            {
                var regs = regRepo.GetByCompanyId(companyId).Where(r => r.Status >= (int)MISA.WorkShift.Core.Enums.RegistrationStatus.Sent);
                if (regs.Any())
                {
                    return BadRequest(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.ValidateError, "Không th? xóa template vì ?ã có t? khai ??ng ký g?i lên C? quan Thu?. Vui lòng ch? xem."));
                }
            }

            // xóa template
            var deleted = _templateRepo.Delete(templateId);
            return Ok(BaseResponse<int>.SuccessResponse(deleted, deleted > 0 ? "Xóa thành công." : "Không tìm th?y template ?? xóa."));
        }
    }
}
