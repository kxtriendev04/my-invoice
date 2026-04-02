using MISA.WorkShift.Core.Entities;
using System;
using System.Threading.Tasks;

namespace MISA.WorkShift.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface định nghĩa các thao tác dữ liệu cho xác thực.
    /// Nằm ở lớp Core để AuthService có thể sử dụng.
    /// </summary>
    public interface IAuthRepository
    {
        /// <summary>
        /// Lấy thông tin User theo Email.
        /// </summary>
        Task<User?> GetUserByEmail(string email);

        /// <summary>
        /// Lấy thông tin Công ty theo Mã số thuế.
        /// </summary>
        Task<Company?> GetCompanyByTaxCode(string taxCode);

        /// <summary>
        /// Kiểm tra User có thuộc Công ty không.
        /// </summary>
        Task<bool> IsUserMemberOfCompany(Guid userId, Guid companyId);

        Task<bool> IsEmailRegistered(string email);
        Task<bool> IsTaxCodeRegistered(string taxCode);
        Task<int> RegisterUserAndCompany(Company company, User user, CompanyUser companyUser);
    }
}