import axiosClient from './axiosClient'

const invoicesApi = {
  // ... other invoice related APIs

  /**
   * Ký và phát hành hóa đơn điện tử.
   * @param {string} invoiceId ID của hóa đơn
   * @param {object} data Dữ liệu chứa SignedXmlContent
   */
  signAndPublish: (invoiceId, data) =>
    axiosClient.post(`/invoices/${invoiceId}/sign-and-publish`, data),
}

export default invoicesApi
