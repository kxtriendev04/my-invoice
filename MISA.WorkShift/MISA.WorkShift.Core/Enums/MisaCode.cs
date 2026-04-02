using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WorkShift.Core.Enums
{
    /// <summary>
    /// Enum định nghĩa các mã trạng thái nghiệp vụ (MISA Code) trả về cho Client.
    /// Dùng để thống nhất cách xử lý kết quả API trên toàn hệ thống.
    /// createdby: kxtrien - 01.12.2025
    /// </summary>
    public enum MisaCode
    {
        /// <summary>
        /// Xử lý thành công (Tương ứng HTTP 200).
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        Success = 200,

        /// <summary>
        /// Tạo mới dữ liệu thành công (Tương ứng HTTP 201).
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        Created = 201,

        /// <summary>
        /// Lỗi dữ liệu đầu vào không hợp lệ (Validate sai - Tương ứng HTTP 400).
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        ValidateError = 400,

        /// <summary>
        /// Lỗi hệ thống nội bộ (Exception - Tương ứng HTTP 500).
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        ServerError = 500,

        /// <summary>
        /// Không tìm thấy dữ liệu (Tương ứng HTTP 404).
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        NotFound = 404,

        /// <summary>
        /// Lỗi xung đột dữ liệu, thường là trùng mã (Tương ứng HTTP 409).
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        DuplicateCode = 409
    }
}
