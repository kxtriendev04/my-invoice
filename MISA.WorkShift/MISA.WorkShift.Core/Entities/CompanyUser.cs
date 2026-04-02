using System;

namespace MISA.WorkShift.Core.Entities
{
    public class CompanyUser
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
    }
}