using MISA.WorkShift.Core.Entities;
using System;
using System.Collections.Generic;

namespace MISA.WorkShift.Core.Interfaces.Repositories
{
    public interface IInvoiceTemplateRepository : IBaseRepository<InvoiceTemplate>
    {
        IEnumerable<InvoiceTemplate> GetByCompanyId(Guid companyId);
        /// <summary>
        /// L?y template theo TemplateId
        /// </summary>
        InvoiceTemplate GetById(Guid templateId);
    }
}
