<script setup>
import { ref, onMounted, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { ArrowLeft, CircleHelp, PlayCircle, Lightbulb } from 'lucide-vue-next'
import invoiceTemplateApi from '@/api/invoiceTemplateApi'
import { BE_URL } from '@/api/axiosClient'

const router = useRouter()

const templates = ref([])
const selectedTemplateCode = ref(null)
const isLoading = ref(false)

const filters = reactive({
  category: 1, // TemplateCategoryEnum.Invoice = 1
  invoiceType: 1, // TemplateInvoiceTypeEnum.Basic = 4 (Mẫu cơ bản)
  size: 1, // PaperSizeEnum.A4 = 1
  taxRate: '', // Để rỗng để load tất cả mẫu mặc định ban đầu
  search: '',
})

/**
 * Gọi API lấy danh sách mẫu mặc định
 */
const fetchTemplates = async () => {
  try {
    isLoading.value = true
    const res = await invoiceTemplateApi.getDefaults(filters)
    if (res && res.data) {
      templates.value = res.data
      if (templates.value.length > 0) {
        selectedTemplateCode.value = templates.value[0].templateCode
      }
    }
  } catch (error) {
    console.error('Lỗi lấy danh sách mẫu:', error)
  } finally {
    isLoading.value = false
  }
}

const goBack = () => {
  router.back()
}

const handleSelectTemplate = (id) => {
  selectedTemplateCode.value = id
}

/**
 * Xử lý khi nhấn "Tiếp tục" hoặc chọn mẫu: Khởi tạo mẫu và sang trang Editor
 */
const handleNext = async (code) => {
  router.push({
    path: '/invoice/template/editor',
    query: {
      code: code,
    },
  })
}

onMounted(() => {
  fetchTemplates()
})
</script>

<template>
  <div class="setup-page">
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

    <main class="page-content">
      <div class="filter-bar">
        <div class="select-group">
          <select class="ms-select" v-model="filters.category" @change="fetchTemplates">
            <option :value="1">Hóa đơn</option>
            <option :value="2">Vé</option>
            <option :value="3">Phiếu thu</option>
          </select>
          <select class="ms-select" v-model="filters.invoiceType" @change="fetchTemplates">
            <option :value="4">Mẫu cơ bản</option>
            <option :value="1">Hóa đơn VAT</option>
          </select>
          <select class="ms-select" v-model="filters.taxRate" @change="fetchTemplates">
            <option value="">Tất cả thuế suất</option>
            <option value="1">Một thuế suất</option>
            <option value="2">Nhiều thuế suất</option>
          </select>
          <select class="ms-select" v-model="filters.size" @change="fetchTemplates">
            <option :value="1">A4</option>
            <option :value="2">A5</option>
          </select>
        </div>
        <div class="hint-box">
          <Lightbulb class="icon-hint" size="18" />
          <span
            >Toàn bộ mẫu đã cho phép thiết lập hiển thị song ngữ trong mục thiết lập chung.</span
          >
        </div>
      </div>

      <div class="template-grid" v-if="!isLoading">
        <div
          v-for="item in templates"
          :key="item.templateCode"
          class="template-card-wrapper"
          @click="handleSelectTemplate(item.templateCode)"
          @dblclick="handleNext(item.templateCode)"
        >
          <div
            v-if="item?.previewRelativePath"
            class=""
            style="position: relative; overflow: hidden"
          >
            <div class="ribbon">Khuyên dùng</div>
            <img
              class="template-card-img"
              :class="{ 'is-selected': selectedTemplateCode === item.templateCode }"
              :src="BE_URL + item.previewRelativePath"
              alt=""
              style="max-width: 333px"
            />
          </div>

          <div v-else class="template-card">
            <div v-if="item.isRecommended" class="ribbon-wrapper">
              <div class="ribbon">Khuyên dùng</div>
            </div>
            <div class="invoice-preview-mockup">
              <div class="mock-header">
                <div class="mock-logo"><b>MyInvoice</b></div>
                <div class="mock-title">HÓA ĐƠN GIÁ TRỊ GIA TĂNG</div>
              </div>
              <div class="mock-body">
                <div class="mock-line"></div>
                <div class="mock-line short"></div>
                <div class="mock-table">
                  <div class="mock-th"></div>
                  <div class="mock-td"></div>
                </div>
                <div class="mock-sig">
                  <div class="sig-box"></div>
                  <div class="sig-box valid">
                    <span style="color: red; font-size: 8px">Signature Valid</span><br />
                    <span style="color: green; font-size: 20px">✓</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="template-label">{{ item.displayName }}</div>
        </div>
      </div>
    </main>
  </div>
</template>

<style scoped>
.setup-page {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background-color: #f4f5f8;
  font-family: Arial, sans-serif;
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
.page-content {
  padding: 16px 24px;
  flex: 1;
  overflow-y: auto;
}
.filter-bar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 6px 0;
  border-radius: 4px;
  margin-bottom: 24px;
}
.select-group {
  display: flex;
  gap: 12px;
}
.ms-select {
  padding: 8px 32px 8px 12px;
  border: 1px solid #d1d5db;
  border-radius: 4px;
  font-size: 13px;
  outline: none;
  background: #fff;
  min-width: 140px;
}
.hint-box {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
}
.icon-hint {
  color: #f5a623;
  fill: #fef0db;
}
.template-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 24px;
}
.template-card-wrapper {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12px;
}
.template-card {
  width: 100%;
  aspect-ratio: 1 / 1.35;
  background: #fff;
  border: 1px solid #e0e0e0;
  border-radius: 4px;
  padding: 10px;
  cursor: pointer;
  position: relative;
  overflow: hidden;
  transition: all 0.2s;
}
.template-card-img.is-selected {
  border: 2px solid #2563eb;
  box-shadow: 0 0 0 1px #2563eb;
}
.ribbon-wrapper {
  position: absolute;
  top: 0;
  right: 0;
  width: 100px;
  height: 100px;
  overflow: hidden;
  z-index: 10;
}
.ribbon {
  position: absolute;
  top: 20px;
  right: -30px;
  width: 140px;
  background: #ff6600;
  color: white;
  text-align: center;
  font-size: 11px;
  font-weight: bold;
  padding: 4px 0;
  transform: rotate(45deg);
}
.template-label {
  font-size: 14px;
  font-weight: bold;
  color: #333;
}
.invoice-preview-mockup {
  width: 100%;
  height: 100%;
  border: 4px double #8cb4e2;
  padding: 10px;
  display: flex;
  flex-direction: column;
  pointer-events: none;
}
.mock-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 10px;
  border-bottom: 1px solid #eee;
  padding-bottom: 5px;
}
.mock-logo {
  font-size: 14px;
  color: #d0021b;
}
.mock-title {
  font-size: 10px;
  font-weight: bold;
  margin-top: 5px;
}
.mock-body {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.mock-line {
  height: 4px;
  background: #f0f0f0;
  width: 100%;
}
.mock-line.short {
  width: 60%;
}
.mock-table {
  flex: 1;
  border: 1px solid #eee;
  margin-top: 10px;
  display: flex;
  flex-direction: column;
}
.mock-th {
  height: 15px;
  background: #fafafa;
  border-bottom: 1px solid #eee;
}
.mock-td {
  height: 15px;
  border-bottom: 1px dashed #f5f5f5;
}
.mock-sig {
  display: flex;
  justify-content: space-between;
  margin-top: 10px;
  height: 60px;
}
.sig-box {
  width: 45%;
  border-top: 1px dashed #ccc;
  text-align: center;
  padding-top: 5px;
}
.sig-box.valid {
  border: 1px solid #c8e6c9;
  background: #f1f8e9;
  justify-content: center;
  align-items: center;
  border-top: 1px solid #c8e6c9;
}
</style>
