using Microsoft.EntityFrameworkCore;
using MISA.WorkShift.Core.Entities;
using MISA.WorkShift.Core.Interfaces.Repositories;
using MISA.WorkShift.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MISA.WorkShift.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Company?> GetCompanyByTaxCode(string taxCode)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.CompanyTaxCode == taxCode);
        }

        public async Task<bool> IsUserMemberOfCompany(Guid userId, Guid companyId)
        {
            return await _context.CompanyUsers.AnyAsync(cu => cu.UserId == userId && cu.CompanyId == companyId);
        }

        public async Task<bool> IsEmailRegistered(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> IsTaxCodeRegistered(string taxCode)
        {
            return await _context.Companies.AnyAsync(c => c.CompanyTaxCode == taxCode);
        }

        public async Task<int> RegisterUserAndCompany(Company company, User user, CompanyUser companyUser)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Companies.Add(company);
                _context.Users.Add(user);
                _context.CompanyUsers.Add(companyUser);
                var res = await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return res;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}