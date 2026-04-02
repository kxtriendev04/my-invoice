using MISA.WorkShift.Core.DTOs;
using MISA.WorkShift.Core.Entities;
using System.Threading.Tasks;

namespace MISA.WorkShift.Core.Interfaces.Services
{
    public interface IInvoiceRegistrationService : IBaseService<InvoiceRegistration>
    {
        Task<string> SaveFullRegistration(RegistrationSaveDto dto);
        
        IEnumerable<InvoiceRegistration> GetByCompanyId(Guid companyId);
        Task<bool> ApproveRegistrationFromCqt(Guid id);
        Task<bool> ApproveInvoiceFromCqt(Guid id);
    }
}