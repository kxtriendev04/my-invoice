using System;

namespace MISA.WorkShift.Core.Entities
{
    public class RegDigitalSignature
    {
        public Guid SignatureId { get; set; }
        public Guid RegistrationId { get; set; }
        public string OrganizationName { get; set; }
        public string SerialNumber { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int ActionType { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}