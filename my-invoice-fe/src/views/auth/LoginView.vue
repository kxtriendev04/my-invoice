<template>
  <div class="misa-login-layout">
    <header class="login-header">
      <div class="container header-container">
        <div class="logo">My Invoice</div>

        <nav class="main-nav">
          <a href="#">Trang chủ</a>
          <a href="#">Tính năng</a>
          <a href="#">Tra cứu</a>
          <a href="#">Tin tức</a>
          <a href="#">Báo giá</a>
          <a href="#">Văn bản</a>
          <a href="#">Hướng dẫn</a>
        </nav>

        <div class="header-actions">
          <button class="btn-consult">
            Tư vấn sử dụng <ChevronDown :size="16" class="ml-4" />
          </button>
        </div>
      </div>
    </header>

    <main class="login-banner">
      <div class="language-switch"><Globe :size="16" class="mr-4" /> Tiếng Việt</div>

      <div class="container banner-container">
        <div class="banner-left">
          <h2 class="subtitle">Phần mềm hóa đơn điện tử</h2>
          <h1 class="title">MyInvoice</h1>
          <p class="tagline">DỄ DÙNG - TIẾT KIỆM - AN TOÀN</p>
        </div>

        <div class="banner-right">
          <div class="login-card">
            <div class="card-logo-wrapper">
              <span>MyInvoice</span>
              <p class="logo-slogan">Phần mềm xử lý hoá đơn</p>
            </div>

            <form @submit.prevent="handleLogin" class="login-form">
              <div class="form-group">
                <input
                  type="text"
                  v-model="formData.taxCode"
                  class="form-control"
                  placeholder="Mã số thuế"
                />
              </div>

              <div class="form-group">
                <input
                  type="email"
                  v-model="formData.email"
                  class="form-control"
                  name="email"
                  autocomplete="username"
                  placeholder="Email đăng nhập"
                />
              </div>

              <div class="form-group input-wrapper">
                <input
                  :type="showPassword ? 'text' : 'password'"
                  v-model="formData.password"
                  class="form-control"
                  name="password"
                  autocomplete="current-password"
                  placeholder="Mật khẩu"
                />
                <button type="button" class="btn-toggle-pass" @click="showPassword = !showPassword">
                  <EyeOff v-if="!showPassword" :size="18" />
                  <Eye v-else :size="18" />
                </button>
              </div>

              <button type="submit" class="btn-login" :disabled="isLoading">
                {{ isLoading ? 'Đang xử lý...' : 'Đăng nhập' }}
              </button>

              <div class="form-links">
                <a href="#" class="link-blue">Quên mật khẩu?</a>
                <a href="#" class="link-blue">Đăng nhập tài khoản khác</a>
              </div>
            </form>
          </div>

          <div class="under-card-text">Một tài khoản sử dụng tất cả các ứng dụng</div>
        </div>
      </div>
    </main>

    <footer class="login-footer">
      <div class="container footer-container">
        <div class="footer-col-contact">
          <div class="footer-logo">
            <!-- <img src="https://cdn.misa.vn/misa-logo.png" alt="MISA" /> -->
            <p>TIN CẬY - TIỆN ÍCH - TẬN TÌNH</p>
          </div>
          <h3 class="company-name">CÔNG TY CỔ PHẦN MYINVOICE</h3>

          <ul class="contact-list">
            <li><Mail :size="16" class="contact-icon" /> contact@myinvoice.vn</li>
            <li><Phone :size="16" class="contact-icon" /> 0243 795 95 95</li>
            <li><Globe :size="16" class="contact-icon" /> https://www.myinvoice.vn/</li>
          </ul>

          <div class="social-icons">
            <div class="social-btn facebook">f</div>
            <div class="social-btn youtube">▶</div>
          </div>
        </div>

        <div class="footer-col-info">
          <ul class="legal-list">
            <li>
              <span class="fw-bold">Trụ sở chính:</span> Tầng 9, tòa nhà Technosoft, phố Duy Tân,
              phường Cầu Giấy, TP. Hà Nội
            </li>
            <li>
              <span class="fw-bold">Giấy CNĐKKD:</span> 0101243150 - Ngày cấp: 22/4/2002, được sửa
              đổi lần thứ 14 ngày 23/7/2018
            </li>
            <li>
              <span class="fw-bold">Cơ quan cấp:</span> Phòng Đăng ký kinh doanh - Sở Kế hoạch và
              Đầu tư TP. Hà Nội
            </li>
          </ul>
        </div>

        <div class="footer-col-certs">
          <div class="certs-images">
            <div class="cert-placeholder">ISO</div>
            <div class="cert-placeholder">ISO</div>
            <div class="cert-placeholder">✔</div>
            <div class="cert-placeholder">DMCA</div>
          </div>
        </div>
      </div>
    </footer>

    <div class="floating-chat-btn">
      <MessageCircle :size="28" color="white" />
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { ChevronDown, Globe, EyeOff, Eye, Mail, Phone, MessageCircle } from 'lucide-vue-next'
import authApi from '@/api/authApi'

const router = useRouter()
const showPassword = ref(false)
const isLoading = ref(false)

const formData = reactive({
  taxCode: '',
  email: '',
  password: '',
})

const handleLogin = async () => {
  if (!formData.taxCode || !formData.email || !formData.password) {
    alert('Vui lòng nhập đầy đủ thông tin đăng nhập!')
    return
  }

  try {
    isLoading.value = true
    const res = await authApi.login(formData)
    console.log('Login response:', res)

    // Axios interceptor unwraps response.data, so handle both structures
    const tokenData = res.data || res
    if (tokenData && tokenData.token) {
      // Lưu vào localStorage
      localStorage.setItem('token', tokenData.token)
      localStorage.setItem('user', JSON.stringify(tokenData.user))
      localStorage.setItem('company', JSON.stringify(tokenData.company))

      router.push('/')
    } else {
      alert('Đăng nhập thất bại: Không nhận được token từ server.')
    }
  } catch (error) {
    console.error('Login Error:', error)
    const errorMsg =
      error.response?.data?.message ||
      error.message ||
      'Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.'
    alert(errorMsg)
  } finally {
    isLoading.value = false
  }
}
</script>

<style scoped>
/* ==========================================
   RESET & BIẾN CƠ BẢN
   ========================================== */
* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

.misa-login-layout {
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  color: #333;
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  background-color: #fff;
}

.container {
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 16px;
}

.fw-bold {
  font-weight: 600;
}
.mr-4 {
  margin-right: 4px;
}
.ml-4 {
  margin-left: 4px;
}

/* ==========================================
   HEADER
   ========================================== */
.login-header {
  height: 64px;
  background-color: #fff;
  border-bottom: 1px solid #eaeaea;
  display: flex;
  align-items: center;
  position: relative;
  z-index: 10;
}

.header-container {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.logo {
  font-size: 18px;
  font-weight: bold;
}

.main-nav {
  display: flex;
  gap: 24px;
}

.main-nav a {
  text-decoration: none;
  color: #111;
  font-size: 14px;
  font-weight: 500;
  transition: color 0.2s;
}

.main-nav a:hover {
  color: #005aab;
}

.header-actions .btn-consult {
  background-color: #1c75db;
  color: #fff;
  border: none;
  padding: 8px 16px;
  border-radius: 20px;
  font-size: 14px;
  font-weight: 500;
  display: flex;
  align-items: center;
  cursor: pointer;
}

.header-actions .btn-consult:hover {
  background-color: #145db0;
}

/* ==========================================
   BANNER & FORM AREA (NỀN CHUYỂN SẮC)
   ========================================== */
.login-banner {
  background-image: url('/login-img.png');
  padding: 60px 0 80px 0;
  position: relative;
  flex: 1;
  background-size: cover;
  background-repeat: no-repeat;
  background-position: center center;
  align-items: center;
  padding-bottom: 30px;
}

.language-switch {
  position: absolute;
  top: 24px;
  right: 40px;
  color: #fff;
  font-size: 14px;
  display: flex;
  align-items: center;
  cursor: pointer;
}

.banner-container {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

/* Slogan Trái */
.banner-left {
  color: #fff;
  flex: 1;
  padding-right: 40px;
}

.banner-left .subtitle {
  font-size: 24px;
  font-weight: 400;
  margin-bottom: 8px;
}

.banner-left .title {
  font-size: 48px;
  font-weight: 700;
  margin-bottom: 16px;
}

.banner-left .tagline {
  font-size: 20px;
  text-transform: uppercase;
}

/* Form Phải */
.banner-right {
  width: 420px;
}

.login-card {
  background: #fff;
  border-radius: 4px;
  padding: 32px 40px 40px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.card-logo-wrapper {
  text-align: center;
  margin-bottom: 24px;
}
.card-logo-wrapper span {
  font-size: 18px;
  font-weight: bold;
}
.card-logo-wrapper p {
  font-size: 12px;
}

.card-logo {
  height: 28px;
}

.logo-slogan {
  font-size: 10px;
  color: #555;
  margin-top: 4px;
  font-weight: 500;
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.form-group {
  position: relative;
}

.form-control {
  width: 100%;
  height: 40px;
  border: 1px solid #d1d5db;
  border-radius: 4px;
  padding: 0 12px;
  font-size: 14px;
  outline: none;
  font-family: inherit;
  transition: border-color 0.2s;
}

.form-control:focus {
  border-color: #1c75db;
}

.input-wrapper {
  display: flex;
  align-items: center;
}

.btn-toggle-pass {
  position: absolute;
  right: 12px;
  top: 50%;
  transform: translateY(-50%);
  background: none;
  border: none;
  color: #9ca3af;
  cursor: pointer;
  display: flex;
  align-items: center;
}

.btn-login {
  background-color: #38a50b; /* Màu xanh lá chuẩn MISA */
  color: #fff;
  border: none;
  height: 44px;
  border-radius: 4px;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  margin-top: 8px;
  transition: background-color 0.2s;
}

.btn-login:hover {
  background-color: #2c8508;
}

.form-links {
  display: flex;
  justify-content: space-between;
  margin-top: 8px;
}

.link-blue {
  color: #1c75db;
  text-decoration: none;
  font-size: 13px;
}

.link-blue:hover {
  text-decoration: underline;
}

.under-card-text {
  text-align: center;
  color: #fff;
  font-size: 13px;
  margin-top: 20px;
}

/* ==========================================
   FOOTER
   ========================================== */
.login-footer {
  background-color: #fff;
  padding: 40px 0;
  border-top: 1px solid #eaeaea;
}

.footer-container {
  display: flex;
  justify-content: space-between;
  gap: 40px;
}

.footer-logo img {
  height: 32px;
}

.footer-logo p {
  font-size: 10px;
  color: #555;
  margin-top: 4px;
  font-weight: 500;
}

.company-name {
  font-size: 14px;
  font-weight: 700;
  margin: 16px 0;
  color: #111;
}

.contact-list,
.legal-list {
  list-style: none;
}

.contact-list li,
.legal-list li {
  font-size: 13px;
  color: #333;
  margin-bottom: 8px;
  display: flex;
  align-items: center;
}

.contact-icon {
  margin-right: 8px;
  color: #555;
}

.legal-list {
  margin-top: 50px;
}

.social-icons {
  display: flex;
  gap: 12px;
  margin-top: 16px;
}

.social-btn {
  width: 32px;
  height: 32px;
  border-radius: 4px;
  color: #fff;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  cursor: pointer;
}

.facebook {
  background-color: #3b5998;
}
.youtube {
  background-color: #ff0000;
  font-size: 18px;
}

/* Placeholder cho các logo chứng chỉ bên phải góc dưới */
.footer-col-certs {
  display: flex;
  align-items: flex-end;
}

.certs-images {
  display: flex;
  gap: 12px;
}

.cert-placeholder {
  width: 40px;
  height: 40px;
  border: 1px solid #ccc;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 10px;
  font-weight: bold;
  color: #005aab;
}

/* ==========================================
   FLOATING CHAT BUTTON
   ========================================== */
.floating-chat-btn {
  position: fixed;
  bottom: 30px;
  right: 30px;
  width: 56px;
  height: 56px;
  background-color: #00c16e;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 12px rgba(0, 193, 110, 0.4);
  cursor: pointer;
  z-index: 100;
  transition: transform 0.2s;
}

.floating-chat-btn:hover {
  transform: scale(1.05);
}
</style>
