using MISA.WorkShift.Core.Entities;
using MISA.WorkShift.Core.Interfaces.Repositories;
using MISA.WorkShift.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MISA.WorkShift.Infrastructure.Repositories
{
    public class InvoiceTemplateRepository : BaseRepository<InvoiceTemplate>, IInvoiceTemplateRepository
    {
        public InvoiceTemplateRepository(AppDbContext context) : base(context)
        {
        }

        public IEnumerable<InvoiceTemplate> GetByCompanyId(Guid companyId)
        {
            return _context.InvoiceTemplates.Where(t => t.CompanyId == companyId).AsNoTracking().ToList();
        }

        public InvoiceTemplate GetById(Guid templateId)
        {
            return _context.InvoiceTemplates.Find(templateId);
        }
    }
}
