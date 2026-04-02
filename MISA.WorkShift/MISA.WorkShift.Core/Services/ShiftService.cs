using ClosedXML.Excel;
using MISA.WorkShift.Core.DTOs;
using MISA.WorkShift.Core.Entities;
using MISA.WorkShift.Core.Enums;
using MISA.WorkShift.Core.Interfaces.Repositories;
using MISA.WorkShift.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WorkShift.Core.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ cho phân hệ Ca làm việc (Shift).
    /// Kế thừa các phương thức chung từ BaseService.
    /// </summary>
    /// <createdby>kxtrien - 01.12.2025</createdby>
    public class ShiftService : BaseService<Shift>, IShiftService
    {
        private readonly IShiftRepository _shiftRepository;

        /// <summary>
        /// Khởi tạo ShiftService.
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        public ShiftService(IShiftRepository shiftRepository) : base(shiftRepository)
        {
            _shiftRepository = shiftRepository;
        }

        /// <summary>
        /// Kiểm tra trùng mã ca.
        /// </summary>
        /// <param name="shiftCode">Mã ca làm việc cần kiểm tra.</param>
        /// <returns>True nếu mã bị trùng, False nếu mã không bị trùng.</returns>
        /// <createdby>kxtrien - 01.12.2025</createdby>
        public bool CheckDuplicateCode(string shiftCode)
        {
            return _shiftRepository.CheckDuplicateCode(shiftCode);
        }

        /// <summary>
        /// Thêm mới ca làm việc.
        /// Có kiểm tra trùng mã trước khi thêm.
        /// </summary>
        /// <param name="entity">Đối tượng Ca làm việc (Shift) cần thêm mới.</param>
        /// <returns>Số dòng bị ảnh hưởng (thường là 1 nếu thành công).</returns>
        /// <exception cref="Exception">Ném ra ngoại lệ nếu Mã ca làm việc đã tồn tại.</exception>
        /// <createdby>kxtrien - 01.12.2025</createdby>
        public override int Insert(Shift entity)
        {
            var isDuplicate = _shiftRepository.CheckDuplicateCode(entity.ShiftCode);
            if (isDuplicate)
            {
                throw new Exception("Mã ca làm việc đã tồn tại.");
            }

            entity.ShiftId = Guid.NewGuid();
            entity.CreatedDate = DateTime.Now;
            entity.Status = ShiftStatus.Active;
            entity.CreatedBy = "Khúc Xuân Triển"; 

            return base.Insert(entity);
        }

        /// <summary>
        /// Cập nhật thông tin ca làm việc.
        /// Kiểm tra sự tồn tại của bản ghi và trùng mã trước khi cập nhật.
        /// </summary>
        /// <param name="id">ID (Guid) của Ca làm việc cần cập nhật.</param>
        /// <param name="entity">Đối tượng Ca làm việc (Shift) với thông tin mới.</param>
        /// <returns>Số dòng bị ảnh hưởng (thường là 1 nếu thành công).</returns>
        /// <exception cref="Exception">Ném ra ngoại lệ nếu Mã ca làm việc đã tồn tại hoặc không tìm thấy bản ghi.</exception>
        /// <createdby>kxtrien - 01.12.2025</createdby>
        public override int Update(Guid id, Shift entity)
        {
            var isDuplicate = _shiftRepository.CheckDuplicateCode(entity.ShiftCode);
            if (isDuplicate)
            {
                throw new Exception("Mã ca làm việc đã tồn tại.");
            }

            var exists = _shiftRepository.CheckIdExists(id);
            if (!exists) throw new Exception("Không tìm thấy bản ghi.");

            entity.ModifiedDate = DateTime.Now;
            entity.ModifiedBy = "Khúc Triển";

            return base.Update(id, entity);
        }

        /// <summary>
        /// Nhập khẩu dữ liệu từ Excel (Chưa triển khai).
        /// </summary>
        /// <param name="filePath">Đường dẫn tới tệp Excel chứa dữ liệu cần nhập khẩu.</param>
        /// <returns>True nếu nhập khẩu thành công.</returns>
        /// <exception cref="NotImplementedException">Phương thức chưa được triển khai.</exception>
        /// <createdby>kxtrien - 01.12.2025</createdby>
        public bool ImportExcel(string filePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Hàm cập nhật trạng thái hàng loạt
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="status"></param>
        /// <returns>Số dòng bị thay đổi</returns>
        /// <exception cref="Exception"></exception>
        /// createdby: kxtrien - 07.12.2025: Yêu cầu số 1,2,3 phải cập nhật được trạng thái theo nhiều dòng
        public int UpdateStatusMulti(List<Guid> ids, int status)
        {
            if (ids == null || ids.Count == 0)
            {
                throw new Exception("Danh sách ID không được để trống.");
            }
            return _shiftRepository.UpdateStatusMulti(ids, status);
        }

        /// <summary>
        /// Xuất khẩu danh sách ca làm việc ra Excel (.xlsx).
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Mảng byte chứa nội dung của tệp Excel (.xlsx).</returns>
        /// createdby: kxtrien - 08.12.2025
        public byte[] ExportExcel(FilterRequestDto request)
        {
            request.PageSize = int.MaxValue;
            request.PageIndex = 1;

            var result = base.GetPaging(request);
            var data = result.Data;

            // KHỞI TẠO EXCEL
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Danh sách Ca");
            worksheet.Style.Font.FontName = "Times New Roman";

            var titleRange = worksheet.Range("A1:N1");
            titleRange.Merge();
            titleRange.Value = "DANH SÁCH CA LÀM VIỆC";
            titleRange.Style.Font.FontSize = 16;
            titleRange.Style.Font.Bold = true;
            titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            var headers = new string[]
            {
                "STT",
                "Mã ca", "Tên ca",
                "Bắt đầu", "Kết thúc",
                "Nghỉ từ", "Nghỉ đến",
                "Tổng giờ làm", "Tổng giờ nghỉ",
                "Trạng thái",
                "Người tạo", "Ngày tạo",
                "Người sửa", "Ngày sửa"
            };

            int currentRow = 3;
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(currentRow, i + 1);
                cell.Value = headers[i];

                // Style Header
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            currentRow++;
            int stt = 1;

            foreach (var item in data)
            {
                int colIndex = 1;

                // 1. STT
                worksheet.Cell(currentRow, colIndex++).Value = stt++;

                // 2. Mã & Tên
                worksheet.Cell(currentRow, colIndex++).Value = item.ShiftCode;
                worksheet.Cell(currentRow, colIndex++).Value = item.ShiftName;

                // (Bỏ qua Description tại đây)

                // 3. Thời gian làm việc (Format HH:mm)
                worksheet.Cell(currentRow, colIndex++).Value = item.StartTime.ToString("HH:mm");
                worksheet.Cell(currentRow, colIndex++).Value = item.EndTime.ToString("HH:mm");

                // 4. Thời gian nghỉ (Nullable - Cần check HasValue)
                worksheet.Cell(currentRow, colIndex++).Value = item.BreakStart.HasValue ? item.BreakStart.Value.ToString("HH:mm") : "";
                worksheet.Cell(currentRow, colIndex++).Value = item.BreakEnd.HasValue ? item.BreakEnd.Value.ToString("HH:mm") : "";

                // 5. Tổng giờ (Number)
                worksheet.Cell(currentRow, colIndex++).Value = item.WorkingTime;
                worksheet.Cell(currentRow, colIndex++).Value = item.BreakTime ?? 0;

                // 6. Trạng thái
                string statusText = item.Status == ShiftStatus.Active ? "Đang sử dụng" : "Ngừng sử dụng";
                var statusCell = worksheet.Cell(currentRow, colIndex++);
                statusCell.Value = statusText;
                if (item.Status != ShiftStatus.Active) statusCell.Style.Font.FontColor = XLColor.Red;

                // 7. Thông tin Audit
                worksheet.Cell(currentRow, colIndex++).Value = item.CreatedBy;

                // Format ngày tháng năm đầy đủ cho ngày tạo/sửa
                worksheet.Cell(currentRow, colIndex++).Value = item.CreatedDate.HasValue ? item.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm") : "";

                worksheet.Cell(currentRow, colIndex++).Value = item.ModifiedBy;
                worksheet.Cell(currentRow, colIndex++).Value = item.ModifiedDate.HasValue ? item.ModifiedDate.Value.ToString("dd/MM/yyyy HH:mm") : "";

                // Căn giữa cho các cột ngắn (STT, Giờ, Ngày)
                worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                // Căn giữa từ cột 4 (Bắt đầu) đến cột 9 (Tổng giờ nghỉ)
                worksheet.Range(currentRow, 4, currentRow, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                // Căn giữa các cột ngày tháng audit
                worksheet.Cell(currentRow, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(currentRow, 14).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                currentRow++;
            }

            if (data != null && data.Any())
            {
                var dataRange = worksheet.Range(3, 1, currentRow - 1, headers.Length);
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            }

            // Tự động giãn cột
            worksheet.Columns().AdjustToContents();

            // Xuất file
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
