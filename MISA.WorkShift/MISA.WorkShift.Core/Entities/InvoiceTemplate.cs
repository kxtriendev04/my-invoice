using System;

namespace MISA.WorkShift.Core.Entities
{
    public class InvoiceTemplate
    {
        public Guid TemplateId { get; set; }
        public Guid CompanyId { get; set; }
        public string TemplateCode { get; set; }
        public string TemplateName { get; set; }
        public string InvSeries { get; set; }
        public string? CssCustom { get; set; }
        public string? XsltContent { get; set; }
        public bool IsDefault { get; set; } = false;
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}