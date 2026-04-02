using System;

namespace MISA.WorkShift.Core.Entities
{
    public class RegTVAN
    {
        public Guid RegTVANId { get; set; }
        public Guid RegistrationId { get; set; }
        public string TVANName { get; set; }
        public string TVANTaxCode { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}