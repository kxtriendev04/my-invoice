﻿using Microsoft.AspNetCore.Mvc;
using MISA.WorkShift.Core.Constants;
using MISA.WorkShift.Core.DTOs;
using MISA.WorkShift.Core.Enums;
using MISA.WorkShift.Core.Interfaces.Services;

namespace MISA.WorkShift.Api.Controllers
{
    /// <summary>
    /// Controller cơ sở cung cấp các API CRUD chuẩn cho các thực thể.
    /// Các Controller khác sẽ kế thừa từ đây để tái sử dụng logic.
    /// createdby: kxtrien - 01.12.2025
    /// </summary>
    /// <typeparam name="TEntity">Loại thực thể (Entity) quản lý</typeparam>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BaseController<TEntity> : ControllerBase where TEntity : class
    {
        protected readonly IBaseService<TEntity> _baseService;

        /// <summary>
        /// Hàm khởi tạo Controller
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="baseService">Service xử lý nghiệp vụ</param>
        public BaseController(IBaseService<TEntity> baseService)
        {
            _baseService = baseService;
        }

        /// <summary>
        /// Lấy danh sách toàn bộ bản ghi
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <returns>Danh sách các đối tượng TEntity</returns>
        [HttpGet]
        public virtual IActionResult GetAll()
        {
            try
            {
                var data = _baseService.GetAll();
                var response = BaseResponse<IEnumerable<TEntity>>.SuccessResponse(data, ServiceMessage.Success);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết một bản ghi theo ID
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="id">ID của bản ghi</param>
        /// <returns>Đối tượng TEntity tìm được</returns>
        [HttpGet("{id}")]
        public virtual IActionResult GetById(Guid id)
        {
            try
            {
                var entity = _baseService.GetById(id);

                if (entity == null)
                {
                    var err = BaseResponse<TEntity>.ErrorResponse(MisaCode.NotFound, ServiceMessage.NotFound);
                    return StatusCode(404, err);
                }

                var response = BaseResponse<TEntity>.SuccessResponse(entity, ServiceMessage.Success);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Thêm mới một bản ghi vào hệ thống
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="entity">Dữ liệu của bản ghi cần thêm mới</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpPost]
        public virtual IActionResult Insert([FromBody] TEntity entity)
        {
            try
            {
                var rowEffect = _baseService.Insert(entity);
                var res = BaseResponse<int>.CreatedResponse(rowEffect, ServiceMessage.InsertSuccess);
                return StatusCode(201, res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        /// <summary>
        /// Cập nhật thông tin một bản ghi đã tồn tại
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="id">ID của bản ghi cần sửa</param>
        /// <param name="entity">Dữ liệu mới cần cập nhật</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpPut("{id}")]
        public virtual IActionResult Update(Guid id, [FromBody] TEntity entity)
        {
            try
            {
                var rowEffect = _baseService.Update(id, entity);
                var res = BaseResponse<int>.SuccessResponse(rowEffect, ServiceMessage.Success);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        /// <summary>
        /// Xóa một bản ghi khỏi hệ thống theo ID
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="id">ID của bản ghi cần xóa</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpDelete("{id}")]
        public virtual IActionResult Delete(Guid id)
        {
            try
            {
                var rowEffect = _baseService.Delete(id);
                var res = BaseResponse<int>.SuccessResponse(rowEffect, ServiceMessage.Success);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Xóa hàng loạt bản ghi theo danh sách ID
        /// createdby: kxtrien - 08.12.2025
        /// </summary>
        /// <param name="ids">Danh sách ID các bản ghi cần xóa</param>
        /// <returns>Số bản ghi đã bị xóa</returns>
        [HttpDelete("batch")]
        public virtual IActionResult DeleteMulti([FromBody] List<Guid> ids)
        {
            try
            {
                // Gọi xuống Service để xử lý xóa nhiều
                // Lưu ý: Bạn cần đảm bảo IBaseService đã có hàm DeleteMulti
                var rowEffect = _baseService.DeleteMulti(ids);
                var res = BaseResponse<int>.SuccessResponse(rowEffect, "Xóa hàng loạt thành công.");
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }




        /// <summary>
        /// Lấy danh sách bản ghi có phân trang, tìm kiếm và sắp xếp nâng cao
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="request">Thông tin phân trang và bộ lọc</param>
        /// <returns>Kết quả phân trang (PagingResult)</returns>
        [HttpPost("filter")]
        public virtual IActionResult GetPaging([FromBody] FilterRequestDto request)
        {
            try
            {
                var result = _baseService.GetPaging(
                    request
                );

                var response = BaseResponse<PagingResult<TEntity>>.SuccessResponse(result);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Hàm xử lý ngoại lệ chung cho toàn bộ Controller
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="ex">Exception bắt được</param>
        /// <returns>ObjectResult chứa thông tin lỗi</returns>
        protected IActionResult HandleException(Exception ex)
        {
            if (ex is MISA.WorkShift.Core.Exceptions.DuplicateException)
            {
                var err409 = BaseResponse<string>.ErrorResponse(MisaCode.DuplicateCode, ex.Message);
                return StatusCode(409, err409);
            }

            if (ex is MISA.WorkShift.Core.Exceptions.NotFoundException)
            {
                var err404 = BaseResponse<string>.ErrorResponse(MisaCode.NotFound, ex.Message);
                return StatusCode(404, err404);
            }

            var err = BaseResponse<string>.ErrorResponse(MisaCode.ServerError, ex.Message);
            return StatusCode(500, err);
        }
    }
}
