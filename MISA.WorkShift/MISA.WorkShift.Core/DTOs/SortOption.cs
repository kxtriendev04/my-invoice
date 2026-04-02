using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WorkShift.Core.DTOs
{
    /// <summary>
    /// Đối tượng chứa thông tin cấu hình sắp xếp cho một cột.
    /// createdby: kxtrien - 04.12.2025
    /// </summary>
    public class SortOption
    {
        /// <summary>
        /// Tên thuộc tính (cột) cần sắp xếp (Ví dụ: shiftCode, createdDate...).
        /// createdby: kxtrien - 04.12.2025
        /// </summary>
        public string ColProp { get; set; }

        /// <summary>
        /// Kiểu sắp xếp.
        /// Các giá trị hợp lệ: "asc" (Tăng dần), "desc" (Giảm dần).
        /// createdby: kxtrien - 04.12.2025
        /// </summary>
        public string Type { get; set; }
    }
}
