using System;

namespace MISA.WorkShift.Core.Entities
{
    public class RegSuspension
    {
        public Guid SuspensionId { get; set; }
        public Guid RegistrationId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}