using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WorkShift.Core.DTOs
{
    /// <summary>
    /// Đối tượng chứa kết quả phân trang chuẩn trả về cho Client.
    /// Bao gồm tổng số bản ghi và dữ liệu của trang hiện tại.
    /// createdby: kxtrien - 01.12.2025
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của bản ghi (Entity hoặc DTO)</typeparam>
    public class PagingResult<T>
    {
        /// <summary>
        /// Tổng số bản ghi thỏa mãn điều kiện lọc (dùng để tính toán chia trang ở Frontend).
        /// Ví dụ: Tìm thấy 100 kết quả, hiển thị trang 1 (20 dòng) -> TotalRecords = 100.
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Danh sách dữ liệu của trang hiện tại.
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        public IEnumerable<T> Data { get; set; }
    }
}
