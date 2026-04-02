using MISA.WorkShift.Core.Entities;
using System.Collections.Generic;

namespace MISA.WorkShift.Core.DTOs
{
    /// <summary>
    /// DTO dùng cho nghiệp vụ thêm mới hóa đơn kèm chi tiết.
    /// </summary>
    public class InvoiceCreateDto
    {
        public Invoice Invoice { get; set; }
        public List<InvoiceDetail> Details { get; set; }
    }
}