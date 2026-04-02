import axiosClient from './axiosClient'

const invoiceRegistrationApi = {
  /**
   * Lấy danh sách đăng ký hóa đơn theo công ty
   * @param {Object} params - Các tham số tìm kiếm và phân trang
   * @returns {Promise} Danh sách đăng ký hóa đơn
   */
  getByCompany(params) {
    return axiosClient.get('/InvoiceRegistrations/by-company', { params })
  },

  /**
   * Tạo tờ khai (Lưu trọn bộ bao gồm signatures, tvans...)
   * @param {Object} data - RegistrationSaveDto
   */
  saveFull(data) {
    return axiosClient.post('/InvoiceRegistrations/save-full', data)
  },

  /**
   * Xuất khẩu dữ liệu ra Excel
   * @param {Object} params - Các tham số tìm kiếm
   * @returns {Promise} Blob file Excel
   */
  exportExcel(params) {
    return axiosClient.get('/InvoiceRegistrations/export', {
      params,
      responseType: 'blob',
    })
  },

  /**
   * Xóa một đăng ký hóa đơn
   * @param {string} id - ID của đăng ký hóa đơn
   */
  delete(id) {
    return axiosClient.delete(`/InvoiceRegistrations/${id}`)
  },

  /**
   * Xóa nhiều đăng ký hóa đơn
   * @param {Array<string>} ids - Danh sách ID các đăng ký hóa đơn
   */
  deleteMulti(ids) {
    return axiosClient.delete('/InvoiceRegistrations/multi', { data: ids })
  },

  /**
   * Cập nhật trạng thái nhiều đăng ký hóa đơn
   * @param {Object} payload - { ids: string[], status: number }
   */
  updateStatusMulti(payload) {
    return axiosClient.put('/InvoiceRegistrations/status', payload)
  },

  /**
   * Lấy chi tiết một đăng ký hóa đơn
   * @param {string} id - ID của đăng ký hóa đơn
   */
  getById(id) {
    return axiosClient.get(`/InvoiceRegistrations/${id}`)
  },
  update(id, data) {
    return axiosClient.put(`/InvoiceRegistrations/${id}`, data)
  },
}

export default invoiceRegistrationApi
