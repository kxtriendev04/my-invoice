using System;

namespace MISA.WorkShift.Core.Entities
{
    public class RegTransmissionUnit
    {
        public Guid TransmissionUnitId { get; set; }
        public Guid RegistrationId { get; set; }
        public string UnitName { get; set; }
        public string TaxCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Note { get; set; }
    }
}