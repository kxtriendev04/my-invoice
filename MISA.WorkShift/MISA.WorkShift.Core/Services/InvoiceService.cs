using MISA.WorkShift.Core.Entities;
using MISA.WorkShift.Core.DTOs;
using MISA.WorkShift.Core.Interfaces.Repositories;
using MISA.WorkShift.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace MISA.WorkShift.Core.Services
{
    public class InvoiceService : BaseService<Invoice>, IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly Core.Interfaces.Services.IInvoiceXmlService _xmlService;

        public InvoiceService(IInvoiceRepository invoiceRepository, Core.Interfaces.Services.IInvoiceXmlService xmlService) : base(invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
            _xmlService = xmlService;
        }

        public string RegenerateInvoiceXml(Invoice invoice, List<InvoiceDetail> details)
        {
            if (invoice == null) throw new ArgumentNullException(nameof(invoice));
            return _xmlService.UpdateInvoiceXml(invoice, details);
        }

        public int UpdateInvoiceWithDetails(Invoice invoice, List<InvoiceDetail> details)
        {
            if (invoice == null) throw new ArgumentNullException(nameof(invoice));

            // validate basic business rules
            ValidateBusiness(invoice, false);

            // regenerate xml
            try
            {
                invoice.XmlFilePath = _xmlService.UpdateInvoiceXml(invoice, details);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo file XML hóa đơn: {ex.Message}", ex);
            }

            // persist header + details
            return _invoiceRepository.UpdateInvoiceMasterDetail(invoice, details);
        }

        /// <summary>
        /// Nghiệp vụ thêm mới hóa đơn kèm chi tiết hàng hóa.
        /// </summary>
        public Invoice CreateInvoiceWithDetails(InvoiceCreateDto dto)
        {
            // 1. Validate nghiệp vụ cho Header
            ValidateBusiness(dto.Invoice, true);

            // 2. Xử lý ID và liên kết
            dto.Invoice.InvoiceId = Guid.NewGuid();
            dto.Invoice.CreatedDate = DateTime.Now;
            
            if (dto.Details != null && dto.Details.Any())
            {
                foreach (var detail in dto.Details)
                {
                    detail.DetailId = Guid.NewGuid();
                    detail.InvoiceId = dto.Invoice.InvoiceId; // Gán FK
                    detail.CreatedDate = DateTime.Now;
                }
            }

            // 3. Thực hiện lưu vào DB (Cần triển khai Transaction trong Repository hoặc dùng Unit of Work)
            // Ở đây demo cách gọi Repository xử lý đơn giản
            // 4. Tạo XML representation của hóa đơn và lưu file vào private_storage/invoices/{companyId}/{invoiceId}.xml
            // build and save xml through xml service
            var savedFullPath = string.Empty;
            try
            {
                dto.Invoice.XmlFilePath = _xmlService.UpdateInvoiceXml(dto.Invoice, dto.Details);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo file XML hóa đơn: {ex.Message}", ex);
            }

            try
            {
                return _invoiceRepository.InsertInvoiceMasterDetail(dto.Invoice, dto.Details);
            }
            catch (Exception)
            {
                // nếu lưu DB thất bại => xóa file XML đã lưu để tránh orphan
                try
                {
                    if (!string.IsNullOrEmpty(savedFullPath) && File.Exists(savedFullPath))
                        File.Delete(savedFullPath);
                }
                catch
                {
                    // swallow - không muốn ghi lỗi xoá file làm lỡ lỗi gốc
                }
                throw;
            }
        }

        // Build XML document for invoice; simple structure compatible with existing default XSLT sample
        private XDocument BuildInvoiceXml(Invoice invoice, List<InvoiceDetail> details)
        {
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));

            var root = new XElement("invoice",
                new XElement("number", invoice.InvoiceNumber ?? string.Empty),
                new XElement("date", invoice.InvoiceDate.ToString("yyyy-MM-ddTHH:mm:ss")),
                new XElement("company",
                    new XElement("name", invoice.SellerName ?? string.Empty),
                    new XElement("tax_code", invoice.SellerTaxCode ?? string.Empty),
                    new XElement("address", invoice.SellerAddress ?? string.Empty)
                ),
                new XElement("buyer",
                    new XElement("name", invoice.BuyerName ?? string.Empty),
                    new XElement("tax_code", invoice.BuyerTaxCode ?? string.Empty),
                    new XElement("address", invoice.BuyerAddress ?? string.Empty),
                    new XElement("email", invoice.BuyerEmail ?? string.Empty),
                    new XElement("phone", invoice.BuyerPhone ?? string.Empty)
                ),
                new XElement("summary",
                    new XElement("total_before_tax", invoice.TotalBeforeTax),
                    new XElement("total_discount", invoice.TotalDiscount),
                    new XElement("total_tax", invoice.TotalTaxAmount),
                    new XElement("total_amount", invoice.TotalAmount)
                )
            );

            var items = new XElement("items");
            if (details != null)
            {
                foreach (var d in details)
                {
                    var item = new XElement("item",
                        new XElement("name", d.ItemName ?? string.Empty),
                        new XElement("unit", d.UnitName ?? string.Empty),
                        new XElement("quantity", d.Quantity),
                        new XElement("unit_price", d.UnitPrice),
                        new XElement("discount_rate", d.DiscountRate),
                        new XElement("discount_amount", d.DiscountAmount),
                        new XElement("tax_rate", d.TaxRate ?? 0),
                        new XElement("tax_amount", d.TaxAmount),
                        new XElement("amount", d.Amount)
                    );
                    items.Add(item);
                }
            }

            root.Add(items);
            doc.Add(root);
            return doc;
        }

        /// <summary>
        /// Ghi đè phương thức Validate nghiệp vụ riêng cho Hóa đơn.
        /// </summary>
        protected override void ValidateBusiness(Invoice entity, bool isInsert)
        {
            // 1. Kiểm tra ngày hóa đơn không được lớn hơn ngày hiện tại
            if (entity.InvoiceDate > DateTime.Now)
            {
                throw new Exception("Ngày hóa đơn không được lớn hơn ngày hiện tại.");
            }

            // 2. Nếu là thêm mới, tự động sinh các thông tin mặc định
            if (isInsert)
            {
                entity.InvoiceId = Guid.NewGuid();
                entity.CreatedDate = DateTime.Now;
                
                if (string.IsNullOrEmpty(entity.InvoiceNumber))
                {
                    entity.InvoiceNumber = _invoiceRepository.GetNextInvoiceNumber();
                }
            }
        }

        public async Task<int> SignAndPublishInvoice(Guid id, string signedXmlContent)
        {
            // 1. Lấy thông tin hóa đơn từ DB
            var invoice = _invoiceRepository.GetById(id);
            if (invoice == null) throw new Exception("Không tìm thấy hóa đơn.");

            // 2. Lưu nội dung XML đã ký vào thư mục Storage
            var relativePath = Path.Combine("Storage", "Invoices", invoice.CompanyId.ToString());
            var fullDir = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
            if (!Directory.Exists(fullDir)) Directory.CreateDirectory(fullDir);

            var fileName = $"{id}.xml";
            var fullPath = Path.Combine(fullDir, fileName);
            await File.WriteAllTextAsync(fullPath, signedXmlContent, new System.Text.UTF8Encoding(false));

            // 3. Cập nhật trạng thái sang "Đã ký / Chờ CQT"
            invoice.XmlFilePath = "/" + Path.Combine(relativePath, fileName).Replace("\\", "/");
            invoice.IssueStatus = 2; // Signed
            invoice.CqtStatus = 2;   // Processing
            invoice.SellerSignedDate = DateTime.Now;

            return _invoiceRepository.Update(id, invoice);
        }
    }
}