using MISA.WorkShift.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WorkShift.Core.DTOs
{

    /// <summary>
    /// Lớp bao bọc dữ liệu trả về chuẩn (Wrapper) cho toàn bộ API.
    /// Giúp thống nhất cấu trúc response trả về cho Client.
    /// createdby: kxtrien - 01.12.2025
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của payload (Data)</typeparam>
    public class BaseResponse<T>
    {
        /// <summary>
        /// Mã code nghiệp vụ (Enum) để phân loại trạng thái xử lý
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        public MisaCode Code { get; set; }

        /// <summary>
        /// Dữ liệu trả về (Generic)
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Thông báo mô tả kết quả (Thành công hoặc mô tả lỗi)
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Hàm khởi tạo đối tượng phản hồi
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="code">Mã nghiệp vụ</param>
        /// <param name="message">Thông báo</param>
        /// <param name="data">Dữ liệu kèm theo</param>
        public BaseResponse(MisaCode code, string message, T data)
        {
            Code = code;
            Message = message;
            Data = data;
        }

        /// <summary>
        /// Helper: Trả về kết quả thành công (Success - 200)
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="data">Dữ liệu trả về</param>
        /// <param name="message">Thông báo (mặc định lấy từ Constants)</param>
        /// <returns>Đối tượng BaseResponse thành công</returns>
        public static BaseResponse<T> SuccessResponse(T data, string message = Constants.ServiceMessage.Success)
        {
            return new BaseResponse<T>(MisaCode.Success, message, data);
        }

        /// <summary>
        /// Helper: Trả về kết quả tạo mới thành công (Created - 201)
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="data">Dữ liệu trả về (thường là ID hoặc số dòng)</param>
        /// <param name="message">Thông báo (mặc định lấy từ Constants)</param>
        /// <returns>Đối tượng BaseResponse tạo mới thành công</returns>
        public static BaseResponse<T> CreatedResponse(T data, string message = Constants.ServiceMessage.InsertSuccess)
        {
            return new BaseResponse<T>(MisaCode.Created, message, data);
        }

        /// <summary>
        /// Helper: Trả về kết quả lỗi (Error - 400, 404, 500...)
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="code">Mã lỗi nghiệp vụ (MisaCode)</param>
        /// <param name="message">Thông báo lỗi chi tiết</param>
        /// <returns>Đối tượng BaseResponse chứa thông tin lỗi (Data sẽ là null/default)</returns>
        public static BaseResponse<T> ErrorResponse(MisaCode code, string message)
        {
            return new BaseResponse<T>(code, message, default(T));
        }
    }
}
