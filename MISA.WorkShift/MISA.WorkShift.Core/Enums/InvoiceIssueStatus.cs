namespace MISA.WorkShift.Core.Enums
{
    public enum InvoiceIssueStatus
    {
        New = 1, // Mới tạo
        Signed = 2, // Đã ký (chưa phát hành)
        Issued = 3, // Đã phát hành
        Cancelled = 4 // Đã hủy
    }
}