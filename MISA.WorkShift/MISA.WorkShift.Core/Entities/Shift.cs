using MISA.WorkShift.Core.Enums;
using System;

namespace MISA.WorkShift.Core.Entities
{
    public class Shift
    {
        public Guid ShiftId { get; set; }
        public string ShiftCode { get; set; }
        public string ShiftName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime? BreakStart { get; set; }
        public DateTime? BreakEnd { get; set; }
        public double WorkingTime { get; set; }
        public double? BreakTime { get; set; }
        public string? Description { get; set; }
        public ShiftStatus Status { get; set; } = ShiftStatus.Active;
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}