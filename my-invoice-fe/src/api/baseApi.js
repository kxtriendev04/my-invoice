import axiosClient from './axiosClient'

/**
 * Lớp Base API cung cấp các phương thức CRUD chuẩn.
 * Các lớp API con (như ShiftApi) sẽ kế thừa từ lớp này để tái sử dụng logic.
 * createdby: kxtrien - 01.12.2025
 */
class BaseApi {
  /**
   * Khởi tạo BaseApi với tên Controller tương ứng.
   * @param {string} controller - Tên resource (VD: 'shifts', 'employees')
   * createdby: kxtrien - 01.12.2025
   */
  constructor(controller) {
    this.controller = controller
  }

  /**
   * Lấy danh sách có phân trang, tìm kiếm và lọc nâng cao.
   * URL: POST /api/v1/{controller}/filter
   * @param {Object} params - Object chứa keyword, pageIndex, pageSize, filters...
   * createdby: kxtrien - 04.12.2025
   */
  getFilterPaging(params) {
    // Gọi vào đường dẫn /shifts/filter đã định nghĩa ở Backend
    return axiosClient.post(`/${this.controller}/filter`, params)
  }

  /**
   * Lấy chi tiết bản ghi theo ID.
   * URL: GET /api/v1/{controller}/{id}
   * @param {string} id - ID bản ghi
   * createdby: kxtrien - 01.12.2025
   */
  getById(id) {
    return axiosClient.get(`/${this.controller}/${id}`)
  }

  /**
   * Thêm mới một bản ghi.
   * URL: POST /api/v1/{controller}
   * @param {Object} data - Dữ liệu cần thêm
   * createdby: kxtrien - 01.12.2025
   */
  insert(data) {
    return axiosClient.post(`/${this.controller}`, data)
  }

  /**
   * Cập nhật thông tin bản ghi.
   * URL: PUT /api/v1/{controller}/{id}
   * @param {string} id - ID bản ghi cần sửa
   * @param {Object} data - Dữ liệu mới
   * createdby: kxtrien - 01.12.2025
   */
  update(id, data) {
    return axiosClient.put(`/${this.controller}/${id}`, data)
  }

  /**
   * Xóa một bản ghi theo ID.
   * URL: DELETE /api/v1/{controller}/{id}
   * @param {string} id - ID bản ghi cần xóa
   * createdby: kxtrien - 01.12.2025
   */
  delete(id) {
    return axiosClient.delete(`/${this.controller}/${id}`)
  }

  /**
   * Xóa nhiều bản ghi cùng lúc.
   * URL: DELETE /api/v1/{controller}/batch
   * @param {Array} ids - Mảng chứa các ID cần xóa (VD: ['guid-1', 'guid-2'])
   * createdby: kxtrien - 08.12.2025
   */
  deleteMulti(ids) {
    // Lưu ý: Route phải khớp với Backend là "/batch"
    return axiosClient.delete(`/${this.controller}/batch`, {
      data: ids,
    })
  }
}

export default BaseApi
