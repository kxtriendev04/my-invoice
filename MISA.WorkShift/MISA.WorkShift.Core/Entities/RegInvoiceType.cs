using System;

namespace MISA.WorkShift.Core.Entities
{
    public class RegInvoiceType
    {
        public Guid RegInvoiceTypeId { get; set; }
        public Guid RegistrationId { get; set; }
        public string InvoiceTypeCode { get; set; }
        public bool IsSelected { get; set; } = false;
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}