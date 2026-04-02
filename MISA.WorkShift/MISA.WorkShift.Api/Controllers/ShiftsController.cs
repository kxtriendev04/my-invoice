using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MISA.WorkShift.Core.Constants;
using MISA.WorkShift.Core.DTOs;
using MISA.WorkShift.Core.Entities;
using MISA.WorkShift.Core.Enums;
using MISA.WorkShift.Core.Interfaces.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MISA.WorkShift.Api.Controllers
{
    /// <summary>
    /// Controller quản lý Ca làm việc (Shifts).
    /// Kế thừa các phương thức CRUD cơ bản từ BaseController.
    /// createdby: kxtrien - 01.12.2025
    /// </summary>
    public class ShiftsController : BaseController<Shift>
    {
        private readonly IShiftService _shiftService;

        /// <summary>
        /// Khởi tạo Controller với ShiftService
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        public ShiftsController(IShiftService shiftService) : base(shiftService)
        {
            _shiftService = shiftService;
        }

        /// <summary>
        /// Cập nhật trạng thái cho nhiều bản ghi cùng lúc (Batch Update Status)
        /// createdby: kxtrien - 05.12.2025
        /// </summary>
        /// <param name="dto">Danh sách ID và trạng thái mới cần cập nhật</param>
        /// <returns>Số bản ghi đã cập nhật</returns>
        [HttpPatch("status")]
        public IActionResult UpdateStatusMulti([FromBody] UpdateStatusDto dto)
        {
            try
            {
                var rowEffect = _shiftService.UpdateStatusMulti(dto.Ids, dto.Status);
                var res = BaseResponse<int>.SuccessResponse(rowEffect, "Cập nhật trạng thái thành công.");
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Xuất khẩu danh sách Ca làm việc ra file Excel (.xlsx)
        /// Hỗ trợ xuất theo đúng điều kiện lọc hiện tại.
        /// createdby: kxtrien - 07.12.2025
        /// </summary>
        /// <param name="request">Điều kiện lọc và sắp xếp (FilterRequestDto)</param>
        /// <returns>File Excel dưới dạng Binary Stream</returns>
        [HttpPost("export")]
        public IActionResult ExportExcel([FromBody] FilterRequestDto request)
        {
            try
            {
                var excelData = _shiftService.ExportExcel(request);
                string fileName = $"Danh_sach_ca_lam_viec_{DateTime.Now:ddMMyyyy_HHmmss}.xlsx";
                // Định nghĩa loại file (MIME Type cho Excel .xlsx)
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File(excelData, contentType, fileName);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
