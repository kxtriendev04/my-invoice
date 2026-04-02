import axiosClient from './axiosClient'

const registrationApi = {
  saveFull(data) {
    const url = '/InvoiceRegistrations/save-full'
    return axiosClient.post(url, data)
  },
}

export default registrationApi
