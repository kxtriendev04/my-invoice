import axiosClient from './axiosClient'

const invoiceTemplateApi = {
  /**
   * Lấy danh sách mẫu mặc định từ tài nguyên server
   */
  getDefaults: (params) => {
    return axiosClient.get('/InvoiceTemplates/defaults', { params })
  },

  /**
   * Lấy nội dung chi tiết (XSLT, HTML, Preview) của một mẫu mặc định
   */
  getDefaultPayload: (templateCode) => {
    return axiosClient.get(`/InvoiceTemplates/defaults/${templateCode}/payload`)
  },

  /**
   * Khởi tạo một bản sao mẫu hóa đơn cho người dùng dựa trên mã mẫu
   */
  initByCode: (templateCode) => {
    return axiosClient.post('/InvoiceTemplates/init-by-code', { templateCode })
  },

  /**
   * Lấy danh sách mẫu hóa đơn đã đăng ký của công ty
   */
  getByCompany: (params) => {
    return axiosClient.get('/InvoiceTemplates/by-company', { params })
  },

  /**
   * Khởi tạo instance từ một templateId đã có
   */
  initForUser: (templateId) => {
    return axiosClient.post(`/InvoiceTemplates/${templateId}/init-for-user`)
  },
  create: (payload) => {
    return axiosClient.post('/InvoiceTemplates/create', payload)
  },
  getById: (id) => {
    return axiosClient.get(`/InvoiceTemplates/${id}`)
  },
  update: (id, payload) => {
    return axiosClient.patch(`/InvoiceTemplates/${id}`, payload)
  },
  delete: (id) => {
    return axiosClient.delete(`/InvoiceTemplates/${id}`)
  },
  deleteMulti: (ids) => {
    return axiosClient.delete('/InvoiceTemplates/multi', { data: ids })
  },
}

export default invoiceTemplateApi
