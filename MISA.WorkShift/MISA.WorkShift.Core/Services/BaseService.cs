using MISA.WorkShift.Core.DTOs;
using MISA.WorkShift.Core.Interfaces.Repositories;
using MISA.WorkShift.Core.Interfaces.Services;
using MISA.WorkShift.Core.MISAAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WorkShift.Core.Services
{
    /// <summary>
    /// Service cơ sở triển khai các nghiệp vụ CRUD chung.
    /// Áp dụng mẫu thiết kế Template Method để cho phép lớp con override các bước Validate.
    /// </summary>
    /// <typeparam name="TEntity">Kiểu dữ liệu Entity</typeparam>
    /// <createdby>kxtrien - 01.12.2025</createdby>
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class
    {
        protected readonly IBaseRepository<TEntity> _baseRepository;

        /// <summary>
        /// Khởi tạo BaseService
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="baseRepository">Repository tương ứng</param>
        public BaseService(IBaseRepository<TEntity> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        /// <summary>
        /// Xóa một bản ghi theo ID
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="id">ID bản ghi cần xóa</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        public virtual int Delete(Guid id)
        {
            return _baseRepository.Delete(id);
        }

        /// <summary>
        /// Lấy toàn bộ danh sách bản ghi
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <returns>Danh sách TEntity</returns>
        public virtual IEnumerable<TEntity> GetAll()
        {
            return _baseRepository.GetAll();
        }

        /// <summary>
        /// Lấy chi tiết bản ghi theo ID
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="id">ID bản ghi</param>
        /// <returns>Đối tượng TEntity</returns>
        public virtual TEntity GetById(Guid id)
        {
            return _baseRepository.GetById(id);
        }

        /// <summary>
        /// Thêm mới một bản ghi.
        /// Có thực hiện validate nghiệp vụ trước khi thêm.
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="entity">Dữ liệu cần thêm</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        public virtual int Insert(TEntity entity)
        {
            ValidateBusiness(entity, true);
            return _baseRepository.Insert(entity);
        }

        /// <summary>
        /// Cập nhật thông tin bản ghi.
        /// Có thực hiện validate nghiệp vụ trước khi cập nhật.
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="id">ID bản ghi cần sửa</param>
        /// <param name="entity">Dữ liệu mới</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        public virtual int Update(Guid id, TEntity entity)
        {
            ValidateBusiness(entity, false);
            return _baseRepository.Update(id, entity);
        }

        /// <summary>
        /// Hàm Helper: Tìm tên cột trong Database dựa vào tên Property (CamelCase) thông qua Attribute [ColumnName].
        /// Dùng để map điều kiện lọc/sort từ Frontend gửi xuống thành tên cột SQL chuẩn.
        /// createdby: kxtrien - 04.12.2025
        /// </summary>
        /// <param name="propName">Tên Property (VD: ShiftCode)</param>
        /// <returns>Tên cột Database (VD: shift_code)</returns>
        private string GetColumnNameFromProperty(string propName)
        {
            if (string.IsNullOrEmpty(propName)) return null;

            var propertyInfo = typeof(TEntity).GetProperties()
                .FirstOrDefault(p => p.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));

            if (propertyInfo == null) return null; // Không tìm thấy property nào tên như vậy

            var attribute = propertyInfo.GetCustomAttribute<ColumnNameAttribute>();

            return attribute?.Name ?? propertyInfo.Name;
        }

        /// <summary>
        /// Hàm lấy danh sách phân trang nâng cao (Generic).
        /// Tự động map các điều kiện lọc và sort từ tên Property sang tên cột Database.
        /// createdby: kxtrien - 04.12.2025
        /// </summary>
        /// <param name="request">Request chứa thông tin lọc</param>
        /// <returns>Kết quả phân trang</returns>
        public virtual PagingResult<TEntity> GetPaging(FilterRequestDto request)
        {
            // Tính toán Ofset
            int offset = (request.PageIndex - 1) * request.PageSize;

            // Xử lý Sắp xếp
            var validSorts = new List<SortOption>();
            if (request.SortOptions != null)
            {
                foreach (var option in request.SortOptions)
                {
                    string dbColumn = GetColumnNameFromProperty(option.ColProp);
                    if (!string.IsNullOrEmpty(dbColumn))
                    {
                        string safeType = option.Type?.Trim().ToLower() == "desc" ? "DESC" : "ASC";
                        validSorts.Add(new SortOption { ColProp = dbColumn, Type = safeType });
                    }
                }
            }

            // Xử lý Bộ lọc
            var mappedFilters = new Dictionary<string, FilterCondition>();

            if (request.Filters != null)
            {
                foreach (var item in request.Filters)
                {
                    // Lấy tên cột DB từ tên Property
                    string dbColumn = GetColumnNameFromProperty(item.Key);

                    // Chỉ thêm nếu tìm thấy cột và có toán tử
                    if (!string.IsNullOrEmpty(dbColumn) && !string.IsNullOrEmpty(item.Value.Operator))
                    {
                        mappedFilters.Add(dbColumn, item.Value);
                    }
                }
            }

            //return _baseRepository.GetFilterPaging(keyword, pageSize, offset, validSorts);
            return _baseRepository.GetFilterPaging(request.Keyword, mappedFilters, request.PageSize, offset, validSorts);
        }

        /// <summary>
        /// Hàm validate nghiệp vụ dùng chung.
        /// Các lớp con (như ShiftService) sẽ override hàm này để viết logic kiểm tra riêng (Check trùng, check ngày tháng...).
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="entity">Dữ liệu cần kiểm tra</param>
        /// <param name="isInsert">True nếu là thêm mới, False nếu là cập nhật</param>
        protected virtual void ValidateBusiness(TEntity entity, bool isInsert)
        {
            
        }

        /// <summary>
        /// Xóa nhiều bản ghi cùng lúc.
        /// createdby: kxtrien - 08.12.2025
        /// </summary>
        /// <param name="ids">Danh sách ID các bản ghi cần xóa</param>
        /// <returns>Số bản ghi đã xóa thành công</returns>
        public virtual int DeleteMulti(List<Guid> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                throw new Exception("Danh sách ID cần xóa không được để trống.");
            }
            var res = _baseRepository.DeleteMulti(ids);

            return res;
        }
    }
}
