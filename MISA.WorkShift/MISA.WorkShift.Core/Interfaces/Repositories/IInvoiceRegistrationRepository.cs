using MISA.WorkShift.Core.Entities;
using MISA.WorkShift.Core.DTOs;
using System.Threading.Tasks;

namespace MISA.WorkShift.Core.Interfaces.Repositories
{
    public interface IInvoiceRegistrationRepository : IBaseRepository<InvoiceRegistration>
    {
        /// <summary>
        /// Lưu trọn bộ tờ khai và các bảng liên quan trong một Transaction
        /// </summary>
        Task<int> SaveFullRegistrationAsync(InvoiceRegistration registration, List<RegDigitalSignature> signatures);
        
        /// <summary>
        /// Lấy danh sách tờ khai theo CompanyId
        /// </summary>
        IEnumerable<InvoiceRegistration> GetByCompanyId(Guid companyId);
    }
}