import BaseApi from './baseApi'
import axiosClient from './axiosClient'

/**
 * Lớp API chuyên biệt cho phân hệ Ca làm việc (Shifts).
 * Kế thừa các phương thức CRUD chung từ BaseApi và định nghĩa thêm các API riêng.
 * createdby: kxtrien - 01.12.2025
 */
class ShiftApi extends BaseApi {
  /**
   * Khởi tạo ShiftApi với controller là 'shifts'.
   * createdby: kxtrien - 01.12.2025
   */
  constructor() {
    super('shifts')
  }

  /**
   * Cập nhật trạng thái hàng loạt (Batch Update).
   * URL: PATCH /api/v1/shifts/status
   * @param {Object} data - Payload chứa { ids: [...], status: 1/0 }
   * createdby: kxtrien - 05.12.2025
   */
  updateStatusMulti(data) {
    return axiosClient.patch(`/${this.controller}/status`, data)
  }

  /**
   * Xuất khẩu dữ liệu ra Excel.
   * URL: POST /api/v1/shifts/export
   * @param {Object} payload - Điều kiện lọc hiện tại (FilterRequestDto)
   * createdby: kxtrien - 07.12.2025
   */
  exportExcel(payload) {
    return axiosClient.post(`/${this.controller}/export`, payload, {
      responseType: 'blob',
    })
  }
}

export default new ShiftApi()
