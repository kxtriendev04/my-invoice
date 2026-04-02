using MISA.WorkShift.Core.Entities;
using System.Collections.Generic;

namespace MISA.WorkShift.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface Repository cho Hóa đơn.
    /// </summary>
    public interface IInvoiceRepository : IBaseRepository<Invoice>
    {
        // Thêm các phương thức đặc thù cho Invoice tại đây nếu cần (VD: Lấy số hóa đơn mới nhất)
        string GetNextInvoiceNumber();
        int InsertInvoiceMasterDetail(Invoice invoice, List<InvoiceDetail> details);
        List<InvoiceDetail> GetDetailsByInvoiceId(Guid invoiceId);
        int UpdateInvoiceMasterDetail(Invoice invoice, List<InvoiceDetail> details);
        /// <summary>
        /// Kiểm tra tồn tại hóa đơn theo series trong một công ty
        /// </summary>
        /// <param name="series">Series của hóa đơn</param>
        /// <param name="companyId">CompanyId</param>
        /// <returns>True nếu tồn tại ít nhất 1 hóa đơn</returns>
        bool ExistsBySeries(string series, Guid companyId);
    }
}