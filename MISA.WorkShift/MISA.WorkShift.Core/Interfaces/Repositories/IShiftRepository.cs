using MISA.WorkShift.Core.DTOs;
using MISA.WorkShift.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WorkShift.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface Repository quản lý riêng cho nghiệp vụ Ca làm việc.
    /// Chứa các phương thức đặc thù chỉ có ở Shift mà không có trong BaseRepository.
    /// createdby: kxtrien - 01.12.2025
    /// </summary>
    public interface IShiftRepository : IBaseRepository<Shift>
    {
        /// <summary>
        /// Kiểm tra xem mã ca làm việc đã tồn tại trong hệ thống hay chưa.
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="shiftCode">Mã ca cần kiểm tra</param>
        /// <returns>True nếu đã tồn tại, False nếu chưa có</returns>
        bool CheckDuplicateCode(string shiftCode);

        /// <summary>
        /// Cập nhật trạng thái hoạt động cho nhiều ca làm việc cùng lúc.
        /// createdby: kxtrien - 05.12.2025
        /// </summary>
        /// <param name="ids">Danh sách ID các ca cần cập nhật</param>
        /// <param name="status">Trạng thái mới (0: Ngừng sử dụng, 1: Đang sử dụng)</param>
        /// <returns>Số bản ghi đã cập nhật thành công</returns>
        int UpdateStatusMulti(List<Guid> ids, int status);
    }
}
