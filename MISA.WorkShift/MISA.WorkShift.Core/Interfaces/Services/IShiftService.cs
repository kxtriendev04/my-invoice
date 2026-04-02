using MISA.WorkShift.Core.DTOs;
using MISA.WorkShift.Core.Entities;
using System;
using System.Collections.Generic;

namespace MISA.WorkShift.Core.Interfaces.Services
{
    /// <summary>
    /// Interface Service quản lý nghiệp vụ Ca làm việc.
    /// createdby: kxtrien - 01.12.2025
    /// </summary>
    public interface IShiftService : IBaseService<Shift>
    {
        /// <summary>
        /// Nhập khẩu danh sách ca làm việc từ file Excel.
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="filePath">Đường dẫn file Excel cần import</param>
        /// <returns>True nếu import thành công, False nếu thất bại</returns>
        bool ImportExcel(string filePath);

        /// <summary>
        /// Kiểm tra xem mã ca làm việc đã tồn tại hay chưa.
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="shiftCode">Mã ca cần kiểm tra</param>
        /// <returns>True nếu trùng, False nếu chưa có</returns>
        bool CheckDuplicateCode(string shiftCode); // Sửa tên tham số từ candidateCode -> shiftCode

        /// <summary>
        /// Cập nhật trạng thái hoạt động cho nhiều ca làm việc cùng lúc.
        /// createdby: kxtrien - 05.12.2025
        /// </summary>
        /// <param name="ids">Danh sách ID các ca cần cập nhật</param>
        /// <param name="status">Trạng thái mới (0: Ngừng sử dụng, 1: Đang sử dụng)</param>
        /// <returns>Số bản ghi đã cập nhật thành công</returns>
        int UpdateStatusMulti(List<Guid> ids, int status);

        /// <summary>
        /// Xuất khẩu danh sách ca làm việc ra file Excel theo điều kiện lọc hiện tại.
        /// createdby: kxtrien - 07.12.2025
        /// </summary>
        /// <param name="request">Điều kiện lọc và sắp xếp</param>
        /// <returns>Mảng byte chứa nội dung file Excel</returns>
        byte[] ExportExcel(FilterRequestDto request);
    }
}