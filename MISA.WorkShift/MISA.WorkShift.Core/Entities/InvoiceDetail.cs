using System;

namespace MISA.WorkShift.Core.Entities
{
    public class InvoiceDetail
    {
        public Guid DetailId { get; set; }
        public Guid InvoiceId { get; set; }
        public string ItemName { get; set; }
        public string? UnitName { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal DiscountAmount { get; set; }
        public int? TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}