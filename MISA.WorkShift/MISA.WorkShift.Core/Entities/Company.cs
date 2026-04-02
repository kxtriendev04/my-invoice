using System;

namespace MISA.WorkShift.Core.Entities
{
    public class Company
    {
        public Guid CompanyId { get; set; }
        public string CompanyTaxCode { get; set; }
        public string CompanyName { get; set; }
        public string? Address { get; set; }
        public string? Representative { get; set; }
        public int Status { get; set; } = 1;
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}