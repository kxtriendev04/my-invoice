using System;

namespace MISA.WorkShift.Core.Entities
{
    public class Invoice
    {
        public Guid InvoiceId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? TemplateId { get; set; }
        public int InvoiceType { get; set; }
        public string? TemplateCode { get; set; }
        public string Series { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string? SellerTaxCode { get; set; }
        public string? SellerName { get; set; }
        public string? SellerAddress { get; set; }
        public string? BuyerTaxCode { get; set; }
        public string? BuyerName { get; set; }
        public string? BuyerLegalName { get; set; }
        public string? BuyerAddress { get; set; }
        public string? BuyerEmail { get; set; }
        public string? BuyerPhone { get; set; }
        public string? BuyerBankAccount { get; set; }
        public string? BuyerBankName { get; set; }
        public string? PaymentMethod { get; set; }
        public string CurrencyCode { get; set; } = "VND";
        public decimal ExchangeRate { get; set; } = 1;
        public int PaymentStatus { get; set; } = 0;
        public decimal TotalBeforeTax { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? TotalAmountInWords { get; set; }
        public int IssueStatus { get; set; } = 1;
        public string? CqtCode { get; set; }
        public int? CqtStatus { get; set; }
        public DateTime? SellerSignedDate { get; set; }
        public Guid? ReferenceInvoiceId { get; set; }
        public string? XmlFilePath { get; set; }
        public string? PdfFilePath { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}