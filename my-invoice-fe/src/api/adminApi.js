import BaseApi from './baseApi'

/**
 * API quản lý công ty - Admin
 * createdby: Admin - 01.04.2026
 */
class CompanyApi extends BaseApi {
  constructor() {
    super('companies')
  }

  // Override nếu cần phương thức đặc biệt
}

/**
 * API quản lý người dùng - Admin
 * createdby: Admin - 01.04.2026
 */
class UserApi extends BaseApi {
  constructor() {
    super('users')
  }
}

/**
 * API gán người dùng vào công ty - Admin
 * createdby: Admin - 01.04.2026
 */
class CompanyUserApi {
  /**
   * Gán người dùng vào công ty
   * @param {Object} data - { companyId, userId, roleId }
   */
  assignUserToCompany(data) {
    return import('./axiosClient').then(({ default: axiosClient }) =>
      axiosClient.post('/company-users/assign', data),
    )
  }

  /**
   * Lấy danh sách người dùng theo công ty
   * @param {string} companyId
   */
  getUsersByCompany(companyId) {
    return import('./axiosClient').then(({ default: axiosClient }) =>
      axiosClient.get(`/company-users/company/${companyId}`),
    )
  }

  /**
   * Lấy danh sách công ty theo người dùng
   * @param {string} userId
   */
  getCompaniesByUser(userId) {
    return import('./axiosClient').then(({ default: axiosClient }) =>
      axiosClient.get(`/company-users/user/${userId}`),
    )
  }

  /**
   * Xóa người dùng khỏi công ty
   * @param {Object} data - { companyId, userId }
   */
  removeUserFromCompany(data) {
    return import('./axiosClient').then(({ default: axiosClient }) =>
      axiosClient.delete('/company-users/remove', { data }),
    )
  }
}

// Export singleton instances
export const companyApi = new CompanyApi()
export const userApi = new UserApi()
export const companyUserApi = new CompanyUserApi()

export default {
  companyApi,
  userApi,
  companyUserApi,
}
