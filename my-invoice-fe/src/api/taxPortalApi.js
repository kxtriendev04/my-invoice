import axiosClient from './axiosClient'

const taxPortalApi = {
  // Lấy danh sách tờ khai chờ duyệt
  getPendingRegistrations: () => axiosClient.get('/TaxPortal/registrations/pending'),

  // Phê duyệt tờ khai
  approveRegistration: (id) => axiosClient.post(`/TaxPortal/registrations/${id}/approve`),

  // Lấy danh sách hóa đơn chờ cấp mã
  getPendingInvoices: () => axiosClient.get('/TaxPortal/invoices/pending'),

  // Duyệt cấp mã hóa đơn
  approveInvoice: (id) => axiosClient.post(`/TaxPortal/invoices/${id}/approve`),
}
export default taxPortalApi
