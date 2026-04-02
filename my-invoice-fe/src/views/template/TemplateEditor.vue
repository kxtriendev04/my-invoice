<template>
  <div class="editor-container">
    <header class="page-header">
      <div class="header-left">
        <button class="btn-icon" @click="goBack"><ArrowLeft size="24" /></button>
        <div class="title-group">
          <h2>Thiết lập mẫu hóa đơn</h2>
          <p>Hãy chọn một mẫu hóa đơn phù hợp với đơn vị để tiếp tục</p>
        </div>
      </div>
      <div class="header-right">
        <button class="btn-help"><CircleHelp size="20" /></button>
        <button class="btn-guide"><PlayCircle size="18" />Phim hướng dẫn</button>
      </div>
    </header>

    <div class="main-layout">
      <div class="preview-panel">
        <div class="preview-wrapper">
          <iframe ref="iframeRef" class="invoice-iframe"></iframe>
        </div>
      </div>

      <div class="editor-panel">
        <div class="form-group">
          <label>Tên mẫu <span class="required">*</span></label>
          <input v-model="formData.templateName" type="text" class="form-control" />
        </div>

        <div class="form-group">
          <label>Ký hiệu <span class="required">*</span></label>
          <input
            v-model="formData.invSeries"
            type="text"
            class="form-control"
            placeholder="Ví dụ: 1C23TAA"
            @input="updatePreviewText"
          />
        </div>

        <!-- <div class="form-group">
          <label>Hình thức hóa đơn <span class="required">*</span></label>
          <div class="radio-group">
            <label>
              <input type="radio" v-model="formData.invoiceType" :value="1" />
              HĐ có mã của CQT
            </label>
            <label>
              <input type="radio" v-model="formData.invoiceType" :value="2" />
              HĐ không có mã của CQT
            </label>
          </div>
        </div> -->

        <div class="form-group">
          <label>Logo</label>
          <div class="upload-box" @click="triggerFileInput('logo')">
            <div class="upload-icon">🖼️</div>
            <p class="m-0">Kéo thả tệp hình ảnh logo vào đây hoặc</p>
            <span class="text-primary">Chọn từ máy tính</span>
            <input
              ref="logoInputRef"
              type="file"
              hidden
              accept="image/png, image/jpeg"
              @change="handleImageUpload($event, 'logo')"
            />
          </div>
        </div>

        <div class="form-group">
          <label>Hình nền</label>
          <div class="upload-box" @click="triggerFileInput('bg')">
            <div class="upload-icon">🖼️</div>
            <p class="m-0">Kéo thả tệp hình ảnh nền vào đây hoặc</p>
            <span class="text-primary">Chọn từ máy tính</span>
            <input
              ref="bgInputRef"
              type="file"
              hidden
              accept="image/png, image/jpeg"
              @change="handleImageUpload($event, 'bg')"
            />
          </div>
        </div>

        <div class="actions">
          <button class="btn btn-default" @click="goBack">Quay lại</button>
          <button class="btn btn-primary" @click="saveConfig">Lưu mẫu</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ArrowLeft, CircleHelp, PlayCircle } from 'lucide-vue-next'
import invoiceTemplateApi from '@/api/invoiceTemplateApi'

const router = useRouter()
const route = useRoute()

// --- Refs ---
const iframeRef = ref(null)
const logoInputRef = ref(null)
const bgInputRef = ref(null)
const templateId = ref(route.params.id) // Lấy ID từ URL nếu đang ở chế độ sửa

const company = JSON.parse(localStorage.getItem('company'))
console.log('company: ', company)
// --- State ---
const formData = reactive({
  templateId: null,
  templateName: 'Hóa đơn GTGT',
  invSeries: '',
  invoiceType: 1, // 1: Có mã, 2: Không mã
  logoBase64: null,
  bgBase64: null,
  previewData: {
    taxCode: company.companyTaxCode || '0101234567',
    companyName: company.companyName || 'CÔNG TY TNHH VÍ DỤ',
    address: company.companyAddress || 'Số 1, Đường ABC, Quận X, Hà Nội',
    phone: company.companyPhone || '0123456789',
    invNo: localStorage.getItem('invNo') || '0000001',
  },
})

// --- Methods ---

const goBack = () => {
  router.back()
}

/**
 * 1. Tải nội dung mẫu từ Backend
 */
const loadTemplateData = async () => {
  try {
    let payload = null

    if (templateId.value) {
      // Chế độ CHỈNH SỬA: Lấy thông tin mẫu đã tồn tại
      const res = await invoiceTemplateApi.getById(templateId.value)
      const data = res?.data || res?.Data
      if (data) {
        formData.templateId = data.templateId || templateId.value
        formData.templateName = data.displayName || data.templateName
        formData.invSeries = data.invSeries || '1C23TAA'
        formData.invoiceType = data.invoiceType || 1
        formData.logoBase64 = data.logo || null
        formData.bgBase64 = data.background || null
        // Payload để render iframe (XSLT content)
        payload = data
      }
    } else {
      // Chế độ THÊM MỚI: Lấy mẫu mặc định dựa trên templateCode
      const templateCode = route.query.code
      if (!templateCode) return

      const res = await invoiceTemplateApi.getDefaultPayload(templateCode)
      payload = res?.data || res?.Data

      if (payload) {
        formData.templateName = payload.displayName || payload.DisplayName || ''
        formData.invSeries = '1C23TAA' // Mặc định cho mẫu mới
      }
    }

    if (payload) {
      // Render nội dung vào Iframe
      if (iframeRef.value) {
        const doc = iframeRef.value.contentWindow.document
        doc.open()
        // Lấy nội dung XSLT hoặc HTML từ đúng trường server trả về
        const content =
          payload.xsltContent ||
          payload.XsltContent ||
          (payload.meta && payload.meta.xsltContent) ||
          ''
        doc.write(content)
        doc.close()

        // Nếu có logo/bg cũ thì apply vào iframe
        applyStylesToIframe()

        // Cập nhật text từ localStorage và input vào iframe sau khi render
        setTimeout(updatePreviewText, 200)
      }
    }
  } catch (error) {
    console.error('Lỗi khi tải dữ liệu mẫu từ BE:', error)
  }
}

/**
 * Apply các style như Logo, Background vào Iframe (dùng khi load dữ liệu sửa)
 */
const applyStylesToIframe = () => {
  const doc = iframeRef.value?.contentWindow?.document
  if (!doc) return

  if (formData.logoBase64) {
    const logoDiv = doc.querySelector('.logo-template-content') || doc.getElementById('logo-layer')
    if (logoDiv) logoDiv.style.backgroundImage = `url('${formData.logoBase64}')`
  }
  if (formData.bgBase64) {
    const bgDiv =
      doc.querySelector('.frame-template') ||
      doc.querySelector('.bg-template') ||
      doc.getElementById('bg-layer')
    if (bgDiv) bgDiv.style.backgroundImage = `url('${formData.bgBase64}')`
  }
}

onMounted(() => {
  loadTemplateData()
})

// 1.5 Cập nhật text trực tiếp vào các class định danh trong Iframe
const updatePreviewText = () => {
  const doc = iframeRef.value?.contentWindow?.document
  if (!doc) return

  // Mappings dựa trên các class phổ biến trong template hóa đơn
  const mappings = {
    '.company-name-content': formData.previewData.companyName.toUpperCase(),
    '.tax-code-content': formData.previewData.taxCode,
    '.seller-address-content': formData.previewData.address,
    '.seller-phone-content': formData.previewData.phone,
    '.inv-series-content': formData.invSeries,
    '.inv-no-content': formData.previewData.invNo,
  }

  Object.entries(mappings).forEach(([selector, value]) => {
    const el = doc.querySelector(selector)
    if (el) el.textContent = value
  })
}

// 2. Kích hoạt input file ẩn
const triggerFileInput = (type) => {
  if (type === 'logo') logoInputRef.value.click()
  if (type === 'bg') bgInputRef.value.click()
}

// 3. Xử lý ảnh sang Base64 và tiêm thẳng vào Iframe DOM
const handleImageUpload = (event, type) => {
  const file = event.target.files[0]
  if (!file) return

  const reader = new FileReader()
  reader.onload = (e) => {
    const base64Image = e.target.result
    const doc = iframeRef.value?.contentWindow?.document
    if (!doc) return

    if (type === 'logo') {
      formData.logoBase64 = base64Image
      // Target đúng class từ css của html payload (.logo-template-content)
      const logoDiv =
        doc.querySelector('.logo-template-content') || doc.getElementById('logo-layer')
      if (logoDiv) logoDiv.style.backgroundImage = `url('${base64Image}')`
    } else if (type === 'bg') {
      formData.bgBase64 = base64Image
      // Target đúng class từ css của html payload (.frame-template hoặc .bg-template)
      const bgDiv =
        doc.querySelector('.frame-template') ||
        doc.querySelector('.bg-template') ||
        doc.getElementById('bg-layer')
      if (bgDiv) bgDiv.style.backgroundImage = `url('${base64Image}')`
    }
  }

  reader.readAsDataURL(file)
  // Reset value để có thể chọn lại cùng 1 file nếu cần
  event.target.value = ''
}

// 4. Submit dữ liệu
const saveConfig = async () => {
  if (!formData.templateName || !formData.invSeries) {
    alert('Vui lòng nhập đầy đủ Tên mẫu và Ký hiệu.')
    return
  }

  try {
    // Cấu trúc payload theo yêu cầu của Backend (thường là POST /api/InvoiceTemplate)
    const payload = {
      templateName: formData.templateName,
      invSeries: formData.invSeries,
      invoiceType: formData.invoiceType,
      logo: formData.logoBase64, // Ảnh logo dạng Base64
      background: formData.bgBase64, // Ảnh nền dạng Base64
      templateCode: route.query.code, // Mã mẫu gốc được chọn từ trang trước
    }

    if (formData.templateId) {
      // Cập nhật mẫu đã có
      await invoiceTemplateApi.update(formData.templateId, payload)
      alert('Cập nhật mẫu hóa đơn thành công!')
    } else {
      // Tạo mới mẫu
      await invoiceTemplateApi.create(payload)
      alert('Lưu mẫu hóa đơn thành công!')
    }

    router.push('/invoice/template/list')
  } catch (error) {
    console.error('Lỗi khi lưu mẫu hóa đơn:', error)
    alert('Có lỗi xảy ra khi lưu. Vui lòng kiểm tra lại thông tin.')
  }
}
</script>

<style scoped>
.editor-container {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background-color: #f3f4f6;
  color: #333;
}

.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 24px;
  background: #fff;
  border-bottom: 1px solid #e0e0e0;
}
.header-left {
  display: flex;
  align-items: center;
  gap: 16px;
}
.btn-icon {
  background: transparent;
  border: none;
  cursor: pointer;
  padding: 8px;
  border-radius: 50%;
  display: flex;
}
.btn-icon:hover {
  background-color: #f0f0f0;
}
.title-group h2 {
  margin: 0 0 2px 0;
  font-weight: bold;
  font-size: 20px;
  color: #111;
}
.title-group p {
  margin: 0;
  font-size: 13px;
  color: #666;
}
.header-right {
  display: flex;
  gap: 12px;
}
.btn-help {
  background: transparent;
  border: 1px solid #ccc;
  border-radius: 4px;
  padding: 6px 10px;
  cursor: pointer;
  color: #555;
}
.btn-guide {
  background-color: #00a84e;
  color: white;
  border: none;
  border-radius: 4px;
  padding: 8px 16px;
  font-weight: 600;
  display: flex;
  gap: 8px;
  cursor: pointer;
}

.main-layout {
  display: flex;
  flex: 1;
  overflow: hidden;
}

/* Cột Preview */
.preview-panel {
  flex: 7;
  padding: 20px;
  overflow: auto;
  display: flex;
  justify-content: center;
  background-color: #e5e7eb;
}

.preview-wrapper {
  width: 865px;
  height: 1224px;
  background: #fff;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.invoice-iframe {
  width: 100%;
  height: 100%;
  border: none;
}

/* Cột Editor */
.editor-panel {
  flex: 3;
  background-color: #fff;
  border-left: 1px solid #e5e7eb;
  padding: 20px;
  min-width: 320px;
  display: flex;
  flex-direction: column;
  overflow-y: auto;
}

.form-group {
  margin-bottom: 24px;
}

label {
  display: block;
  font-weight: 500;
  margin-bottom: 8px;
  font-size: 14px;
}

.required {
  color: red;
}

.form-control {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid #d1d5db;
  border-radius: 4px;
  outline: none;
}

.form-control:focus {
  border-color: #3b82f6;
}

.radio-group {
  display: flex;
  gap: 20px;
  font-size: 14px;
}

.radio-group label {
  font-weight: 400;
  display: flex;
  align-items: center;
  gap: 6px;
  cursor: pointer;
}

.upload-box {
  border: 2px dashed #d1d5db;
  border-radius: 8px;
  padding: 24px 20px;
  text-align: center;
  cursor: pointer;
  font-size: 13px;
  color: #6b7280;
  transition: all 0.2s;
}

.upload-box:hover {
  border-color: #3b82f6;
  background-color: #eff6ff;
}

.upload-icon {
  font-size: 24px;
  margin-bottom: 8px;
}

.text-primary {
  color: #2563eb;
}

.m-0 {
  margin: 0;
}

.actions {
  margin-top: auto;
  padding-top: 20px;
  border-top: 1px solid #e5e7eb;
  display: flex;
  justify-content: space-between;
}

.btn {
  padding: 8px 16px;
  border: none;
  border-radius: 4px;
  font-weight: 500;
  cursor: pointer;
}

.btn-default {
  background-color: #fff;
  border: 1px solid #d1d5db;
}

.btn-primary {
  background-color: #2563eb;
  color: white;
}

.btn-primary:hover {
  background-color: #1d4ed8;
}
</style>
