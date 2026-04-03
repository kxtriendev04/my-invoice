using MISA.WorkShift.Core.Entities;
using MISA.WorkShift.Core.DTOs;

namespace MISA.WorkShift.Core.Interfaces.Services
{
    public interface IInvoiceService : IBaseService<Invoice>
    {
        // Các phương thức nghiệp vụ đặc thù cho Invoice
        Invoice CreateInvoiceWithDetails(InvoiceCreateDto dto);
        // Re-generate and overwrite XML for an invoice (returns relative path)
        string RegenerateInvoiceXml(Invoice invoice, System.Collections.Generic.List<InvoiceDetail> details);
        Task<int> SignAndPublishInvoice(Guid id, string signedXmlContent);
        int UpdateInvoiceWithDetails(Invoice invoice, System.Collections.Generic.List<InvoiceDetail> details);
    }
}