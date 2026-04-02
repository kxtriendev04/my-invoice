using MISA.WorkShift.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WorkShift.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface định nghĩa các phương thức giao tiếp với Database chung cho mọi Entity.
    /// createdby: kxtrien - 01.12.2025
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của Entity (VD: Shift, Employee)</typeparam>
    public interface IBaseRepository<T>
    {
        /// <summary>
        /// Lấy toàn bộ danh sách bản ghi.
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <returns>Danh sách các đối tượng T</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Lấy thông tin chi tiết một bản ghi theo ID.
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="id">ID của bản ghi</param>
        /// <returns>Đối tượng T tìm được</returns>
        T GetById(Guid id);

        /// <summary>
        /// Thêm mới một bản ghi vào Database.
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="entity">Dữ liệu cần thêm</param>
        /// <returns>Số dòng bị ảnh hưởng (1 là thành công)</returns>
        int Insert(T entity);

        /// <summary>
        /// Cập nhật thông tin bản ghi.
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="id">ID bản ghi cần sửa</param>
        /// <param name="entity">Dữ liệu mới</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        int Update(Guid id, T entity);

        /// <summary>
        /// Xóa một bản ghi theo ID.
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="id">ID bản ghi cần xóa</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        int Delete(Guid id);

        /// <summary>
        /// Xóa nhiều bản ghi cùng lúc theo danh sách ID.
        /// createdby: kxtrien - 08.12.2025
        /// </summary>
        /// <param name="ids">Danh sách ID các bản ghi cần xóa</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        int DeleteMulti(List<Guid> ids);

        /// <summary>
        /// Kiểm tra xem ID có tồn tại trong hệ thống không.
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="id">ID cần kiểm tra</param>
        /// <returns>True nếu tồn tại, False nếu không</returns>
        bool CheckIdExists(Guid id);

        /// <summary>
        /// Lấy danh sách có phân trang, lọc và sắp xếp (Advanced Filter).
        /// createdby: kxtrien - 04.12.2025
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm chung</param>
        /// <param name="filters">Danh sách điều kiện lọc theo cột</param>
        /// <param name="pageSize">Số bản ghi trên 1 trang (Limit)</param>
        /// <param name="offset">Vị trí bắt đầu lấy (Offset)</param>
        /// <param name="sortOptions">Danh sách tiêu chí sắp xếp</param>
        /// <returns>Đối tượng phân trang chứa dữ liệu và tổng số bản ghi</returns>
        PagingResult<T> GetFilterPaging(string keyword, Dictionary<string, FilterCondition> filters, int pageSize, int offset, List<SortOption> sortOptions);
    }
}
