import axiosClient from './axiosClient'

const invoiceApi = {
  // T?o m?i h�a ??n k�m chi ti?t
  create(payload) {
    return axiosClient.post('/invoices/creation', payload)
  },

  // Ph�t h�nh (publish) h�a ??n
  publish(id) {
    return axiosClient.post(`/invoices/${id}/publish`)
  },

  // L?y file XML (y�u c?u token/authorization)
  getXml(id) {
    return axiosClient.get(`/invoices/${id}/xml`, { responseType: 'blob' })
  },

  // Render invoice -> html or pdf
  render(id, format = 'html') {
    return axiosClient.get(`/invoices/${id}/render`, { params: { format } })
  },
  // L?y danh s�ch ph�n trang, l?c, s?p x?p
  getFilterPaging(params) {
    return axiosClient.post('/invoices/filter', params)
  },

  // Xu?t Excel
  exportExcel(params) {
    return axiosClient.post('/invoices/export', params, { responseType: 'blob' })
  },

  // CRUD c? b?n
  getById(id) {
    return axiosClient.get(`/invoices/${id}`)
  },
  insert(data) {
    return axiosClient.post('/invoices', data)
  },
  update(id, data) {
    return axiosClient.put(`/invoices/${id}/full`, data)
  },
  delete(id) {
    return axiosClient.delete(`/invoices/${id}`)
  },
  deleteMulti(ids) {
    return axiosClient.delete('/invoices/batch', { data: ids })
  },
  // C?p nh?t tr?ng th�i h�ng lo?t
  updateStatusMulti(data) {
    return axiosClient.patch('/invoices/status', data)
  },
}

export default invoiceApi
