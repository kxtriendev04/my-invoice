using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WorkShift.Core.Constants
{
    /// <summary>
    /// Lớp chứa các thông báo chuẩn (Message) trả về từ Service cho Client.
    /// createdby: kxtrien - 01.12.2025
    public class ServiceMessage
    {
        /// <summary>
        /// Thông báo thành công chung
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        public const string Success = "Thành công.";

        /// <summary>
        /// Thông báo khi thêm mới thành công
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        public const string InsertSuccess = "Thêm mới thành công.";

        /// <summary>
        /// Thông báo lỗi hệ thống (Exception không mong muốn)
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        public const string ServerError = "Có lỗi xảy ra vui lòng liên hệ MISA.";

        /// <summary>
        /// Thông báo dữ liệu đầu vào không hợp lệ (Validate sai)
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        public const string BadRequest = "Dữ liệu đầu vào không hợp lệ.";

        /// <summary>
        /// Thông báo khi không tìm thấy bản ghi (GetById, Update, Delete)
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        public const string NotFound = "Không tìm thấy dữ liệu.";

        /// <summary>
        /// Thông báo khi mã đối tượng bị trùng
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        public const string DuplicateCode = "Mã code đã tồn tại trong hệ thống.";
    }
}
