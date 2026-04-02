using MISA.WorkShift.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WorkShift.Core.Interfaces.Services
{
    /// <summary>
    /// Interface cơ sở định nghĩa các nghiệp vụ chung (CRUD) cho các Service.
    /// createdby: kxtrien - 01.12.2025
    /// </summary>
    /// <typeparam name="TEntity">Kiểu thực thể (Entity) mà Service quản lý</typeparam>
    public interface IBaseService<TEntity>
    {
        /// <summary>
        /// Lấy danh sách toàn bộ bản ghi.
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <returns>Danh sách các đối tượng TEntity</returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Lấy thông tin chi tiết một bản ghi theo ID.
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="id">ID của bản ghi cần lấy</param>
        /// <returns>Đối tượng TEntity tìm được</returns>
        TEntity GetById(Guid id);

        /// <summary>
        /// Thêm mới một bản ghi vào hệ thống.
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="entity">Dữ liệu cần thêm mới</param>
        /// <returns>Số bản ghi bị ảnh hưởng (1 là thành công)</returns>
        int Insert(TEntity createDto);

        /// <summary>
        /// Cập nhật thông tin bản ghi.
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="id">ID của bản ghi cần sửa</param>
        /// <param name="entity">Dữ liệu mới cần cập nhật</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        int Update(Guid id, TEntity updateDto);

        /// <summary>
        /// Xóa một bản ghi khỏi hệ thống.
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="id">ID của bản ghi cần xóa</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        int Delete(Guid id);

        /// <summary>
        /// Xóa nhiều bản ghi cùng lúc theo danh sách ID.
        /// createdby: kxtrien - 08.12.2025
        /// </summary>
        /// <param name="ids">Danh sách ID các bản ghi cần xóa</param>
        /// <returns>Số bản ghi đã xóa thành công</returns>
        int DeleteMulti(List<Guid> ids);


        /// <summary>
        /// Lấy danh sách có phân trang, tìm kiếm, lọc nâng cao và sắp xếp.
        /// createdby: kxtrien - 04.12.2025
        /// </summary>
        /// <param name="request">Đối tượng chứa thông tin phân trang và bộ lọc</param>
        /// <returns>Kết quả phân trang (Dữ liệu + Tổng số bản ghi)</returns>
        PagingResult<TEntity> GetPaging(FilterRequestDto request);


    }
}
