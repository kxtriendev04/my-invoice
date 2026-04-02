using MISA.WorkShift.Core.Entities;
using MISA.WorkShift.Core.Interfaces.Repositories;
using MISA.WorkShift.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MISA.WorkShift.Infrastructure.Repositories
{
    public class InvoiceTemplateXsltRepository : IInvoiceTemplateXsltRepository
    {
        private readonly AppDbContext _context;

        public InvoiceTemplateXsltRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Insert(InvoiceTemplateXslt xslt)
        {
            _context.Set<InvoiceTemplateXslt>().Add(xslt);
            _context.SaveChanges();
        }

        public IEnumerable<InvoiceTemplateXslt> GetByUserId(Guid userId)
        {
            return _context.Set<InvoiceTemplateXslt>().Where(x => x.UserId == userId).AsNoTracking().ToList();
        }

        public InvoiceTemplateXslt GetById(Guid instanceId)
        {
            return _context.Set<InvoiceTemplateXslt>().Find(instanceId);
        }

        public IEnumerable<InvoiceTemplateXslt> GetByTemplateId(Guid templateId)
        {
            return _context.Set<InvoiceTemplateXslt>().Where(x => x.TemplateId == templateId).AsNoTracking().ToList();
        }
    }
}
