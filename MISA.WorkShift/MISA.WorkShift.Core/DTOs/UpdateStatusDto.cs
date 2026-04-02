using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WorkShift.Core.DTOs
{
    /// <summary>
    /// DTO chứa thông tin để cập nhật trạng thái cho nhiều bản ghi cùng lúc.
    /// createdby: kxtrien - 05.12.2025
    /// </summary> 
    public class UpdateStatusDto
    {
        /// <summary>
        /// Danh sách các ID bản ghi cần cập nhật trạng thái.
        /// createdby: kxtrien - 05.12.2025
        /// </summary>
        public List<Guid> Ids { get; set; }

        /// <summary>
        /// Trạng thái mới muốn áp dụng (Ví dụ: 0 - Ngừng sử dụng, 1 - Đang sử dụng).
        /// createdby: kxtrien - 05.12.2025
        /// </summary>
        public int Status { get; set; }
    }
}
