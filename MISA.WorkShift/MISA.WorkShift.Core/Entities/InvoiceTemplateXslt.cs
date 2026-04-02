using System;

namespace MISA.WorkShift.Core.Entities
{
    public class InvoiceTemplateXslt
    {
        public Guid InstanceId { get; set; }
        public Guid? TemplateId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public string XsltContent { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
    }
}
