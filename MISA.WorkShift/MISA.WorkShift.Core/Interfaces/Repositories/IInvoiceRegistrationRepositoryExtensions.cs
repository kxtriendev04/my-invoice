using MISA.WorkShift.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MISA.WorkShift.Core.Interfaces.Repositories
{
    public static class IInvoiceRegistrationRepositoryExtensions
    {
        public static IEnumerable<InvoiceRegistration> GetByCompanyId(this IInvoiceRegistrationRepository repo, Guid companyId)
        {
            // Default implementation using underlying DbContext is not accessible here.
            // Concrete repository (InvoiceRegistrationRepository) will implement actual query.
            throw new NotImplementedException("Use concrete repository implementation.");
        }
    }
}
