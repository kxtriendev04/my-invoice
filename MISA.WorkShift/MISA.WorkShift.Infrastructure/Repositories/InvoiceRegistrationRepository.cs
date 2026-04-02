using MISA.WorkShift.Core.Entities;
using MISA.WorkShift.Core.Interfaces.Repositories;
using MISA.WorkShift.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MISA.WorkShift.Infrastructure.Repositories
{
    public class InvoiceRegistrationRepository : BaseRepository<InvoiceRegistration>, IInvoiceRegistrationRepository
    {
        public InvoiceRegistrationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<int> SaveFullRegistrationAsync(InvoiceRegistration registration, List<RegDigitalSignature> signatures)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Lưu Tờ khai (đảm bảo EF đánh dấu là Added)
                _context.Entry(registration).State = Microsoft.EntityFrameworkCore.EntityState.Added;

                // 2. Lưu Chữ ký số (đảm bảo không gây thay đổi khóa chính của registration)
                if (signatures != null && signatures.Any())
                {
                    foreach (var sig in signatures)
                    {
                        if (sig.SignatureId == Guid.Empty) sig.SignatureId = Guid.NewGuid();
                        // gán FK rõ ràng
                        sig.RegistrationId = registration.RegistrationId;
                        // tránh EF cố gắng cập nhật navigation/khóa: đánh dấu là Added
                        _context.Entry(sig).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                        // nếu DbSet được sử dụng, cũng có thể thêm explicit
                        //_context.RegDigitalSignatures.Add(sig);
                    }
                }

                // Lưu các bảng phụ khác nếu có...

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

        protected override string[] SearchableColumns => new string[]
        {
            "taxpayer_name", "tax_code", "registration_no"
        };

        public IEnumerable<InvoiceRegistration> GetByCompanyId(Guid companyId)
        {
            return _context.InvoiceRegistrations.Where(r => r.CompanyId == companyId).AsNoTracking().ToList();
        }
    }
}