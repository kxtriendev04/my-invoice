namespace MISA.WorkShift.Core.Enums
{
    // Category of template
    public enum TemplateCategoryEnum
    {
        Unknown = 0,
        Invoice,
        Ticket,
        Receipt
    }

    // Invoice type / tax structure - renamed to avoid collision with existing types
    public enum TemplateInvoiceTypeEnum
    {
        Unknown = 0,
        VAT,
        SingleRate,
        MultiRate,
        Basic
    }

    // Paper sizes
    public enum PaperSizeEnum
    {
        Unknown = 0,
        A4,
        A5
    }
}
