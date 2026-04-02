using System;

namespace MISA.WorkShift.Core.Entities
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        public Guid CompanyId { get; set; }
        public string? BuyerTaxCode { get; set; }
        public string? BuyerName { get; set; }
        public string? BuyerLegalName { get; set; }
        public string? BuyerAddress { get; set; }
        public string? BuyerEmail { get; set; }
        public string? BuyerPhone { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}