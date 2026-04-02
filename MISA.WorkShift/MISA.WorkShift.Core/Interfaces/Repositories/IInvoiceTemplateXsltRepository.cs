using MISA.WorkShift.Core.Entities;
using System;
using System.Collections.Generic;

namespace MISA.WorkShift.Core.Interfaces.Repositories
{
    public interface IInvoiceTemplateXsltRepository
    {
        void Insert(InvoiceTemplateXslt xslt);
        IEnumerable<InvoiceTemplateXslt> GetByUserId(Guid userId);
        InvoiceTemplateXslt GetById(Guid instanceId);
        IEnumerable<InvoiceTemplateXslt> GetByTemplateId(Guid templateId);
    }
}
