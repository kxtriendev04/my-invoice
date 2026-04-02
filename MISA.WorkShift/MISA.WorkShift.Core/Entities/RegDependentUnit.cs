using System;

namespace MISA.WorkShift.Core.Entities
{
    public class RegDependentUnit
    {
        public Guid DependentUnitId { get; set; }
        public Guid RegistrationId { get; set; }
        public string UnitName { get; set; }
        public string TaxCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}