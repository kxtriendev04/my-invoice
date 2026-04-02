using System;

namespace MISA.WorkShift.Core.Entities
{
    public class InvoiceRegistration
    {
        public Guid RegistrationId { get; set; }
        public Guid CompanyId { get; set; }
        public string? RegistrationNo { get; set; }
        public int TransType { get; set; }
        public string TaxCode { get; set; }
        public string TaxpayerName { get; set; }
        public string? Address { get; set; }
        public string? TaxAuthorityCode { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public string RepresentativeName { get; set; }
        public int? IdentityType { get; set; }
        public string? IdentityNo { get; set; }
        public int? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Nationality { get; set; }
        public string? ContactAddress { get; set; }
        public int InvoiceAppType { get; set; }
        public int? SubjectCategory { get; set; }
        public int DataTransferMode { get; set; }
        public int? MethodOfTransfer { get; set; }
        public string? IssuePlace { get; set; }
        public int Status { get; set; } = 1;
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}