import axios from 'axios'

export const BE_URL = 'https://localhost:7248'
const axiosClient = axios.create({
  baseURL: BE_URL + '/api/v1',
  headers: {
    'Content-Type': 'application/json',
  },
})

// Interceptor để đính kèm Token vào Header của mọi request
axiosClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
      // Fallback: Gửi token qua query param nếu backend không đọc được từ header (CORS preflight issue)
      // config.params = { ...config.params, accessToken: token }
    } else {
      console.warn(
        '⚠️ Không tìm thấy token trong localStorage. Request sẽ không có Authorization header.',
      )
    }
    console.log(`📡 Request: ${config.method?.toUpperCase()} ${config.baseURL}${config.url}`)
    return config
  },
  (error) => {
    return Promise.reject(error)
  },
)

// Interceptor để xử lý lỗi tập trung (ví dụ: Token hết hạn)
axiosClient.interceptors.response.use(
  (response) => {
    return response.data
  },
  (error) => {
    // Chỉ redirect về login nếu thực sự là lỗi 401 (hết hạn token)
    // và người dùng đang không ở trang login
    if (error.response?.status === 401) {
      const currentPath = window.location.pathname
      if (currentPath !== '/login') {
        // Lưu URL hiện tại để có thể redirect lại sau khi login
        const token = localStorage.getItem('token')
        // Chỉ xóa token và redirect nếu trước đó đã có token (đã login)
        if (token) {
          localStorage.removeItem('token')
          localStorage.removeItem('user')
          window.location.href = '/login?redirect=' + encodeURIComponent(currentPath)
          return Promise.reject(error)
        }
      }
    }
    return Promise.reject(error)
  },
)

export default axiosClient
