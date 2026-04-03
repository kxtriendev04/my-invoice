<script setup>
import MSButton from '@/components/ms-button/MSButton.vue'
import MSAlert from '@/components/ms-alert/MSAlert.vue'
import MSPagination from '@/components/ms-pagination/MSPagination.vue'
import MSTable from '@/components/ms-table/MSTable.vue'
import { tableHeader } from '@/data/shiftData'
import { formatDateString } from '@/utils/format'
import { CircleCheck, CircleOff, Plus, Trash2, X } from 'lucide-vue-next'
import { computed, onMounted, onUnmounted, ref, watch, watchEffect } from 'vue'
import invoiceApi from '@/api/invoiceApi'
import axiosClient from '@/api/axiosClient'
import { useRouter } from 'vue-router'
import ShiftUpsertPopUp from './ShiftUpsertPopup.vue'
import { useToastStore } from '@/stores/useToastStore'
import shiftApi from '@/api/shiftApi'
import { processShiftPayload } from '@/utils/shiftHelper'
import * as signalR from '@microsoft/signalr'

//#region Constants
const router = useRouter()
const opMap = {
  // Number
  eq: 'Bằng',
  neq: 'Khác',
  gt: 'Lớn hơn',
  gte: 'Lớn hơn hoặc bằng',
  lt: 'Nhỏ hơn',
  lte: 'Nhỏ hơn hoặc bằng',
  // Text & Date
  contains: 'Chứa',
  startsWith: 'Bắt đầu bằng',
  date_eq: 'Ngày',
  not_contains: 'Không chứa',
  endsWith: 'Kết thúc bằng',
  empty: 'Là trống',
  not_empty: 'Không trống',
}
//#endregion

//#region State
// Popup State
const isOpenPopup = ref(false)
const popupData = ref({})
const toast = useToastStore()
const isLoading = ref(false)
const popupCount = ref(0)
let connection = null

// Pagination State
const itemsPerPage = ref(20)
const itemsPerPageOptions = ref([10, 20, 30, 50, 100])
const currentPage = ref(1)
const totalRecords = ref(0)

// Search State
const searchKeyword = ref('')

// Table Data State
const shiftColumns = ref(tableHeader)
const displayData = ref([])

// Feature State (Filter, Sort, Select)
const currentFilters = ref({})
const activeSorts = ref([])
const selectedRowIds = ref([])

// Delete State
const isShowAlert = ref(false)
const itemToDelete = ref(null)
const isMultipleDelete = ref(false)
const confirmInvoiceNumber = ref('')

let timeoutSearch = null
//#endregion

//#region Computed
/**
 * Kiểm tra xem có đang áp dụng bộ lọc nào không
 * createdby: kxtrien - 01.12.2025
 */
const hasActiveFilters = computed(() => {
  const filters = Object.values(currentFilters.value || {})
  return filters.some((f) => {
    return f.value !== '' && f.value != null
  })
})

/**
 * Lấy ra danh sách đầy đủ các object đang được chọn
 * createdby: kxtrien - 03.12.2025
 */
const selectedItems = computed(() => {
  return displayData.value.filter((item) => selectedRowIds.value.includes(item.InvoiceId))
})

/**
 * Kiểm tra xem có item nào đang ở trạng thái 'Ngừng sử dụng' (0) để hiện nút 'Sử dụng' không
 * createdby: kxtrien - 03.12.2025
 */
const showBtnActive = computed(() => {
  return selectedItems.value.some((item) => item.status === 0)
})

/**
 * Kiểm tra xem có item nào đang ở trạng thái 'Đang sử dụng' (1) để hiện nút 'Ngừng sử dụng' không
 * createdby: kxtrien - 03.12.2025
 */
const showBtnInactive = computed(() => {
  return selectedItems.value.some((item) => item.status === 1)
})
//#endregion

//#region Watchers
/**
 * debuggger
 */
watchEffect(() => {
  // console.log('Filters:', JSON.parse(JSON.stringify(currentFilters.value)))
  // console.log('activeSorts: ', activeSorts.value)
})

/**
 * Theo dõi thay đổi trang hoặc số dòng/trang để load lại dữ liệu
 * createdby: kxtrien - 01.12.2025
 */
watch([currentPage, itemsPerPage], () => {
  loadData()
})

/**
 * Theo dõi từ khóa tìm kiếm (Debounce 500ms)
 * createdby: kxtrien - 01.12.2025
 */
watch(searchKeyword, () => {
  if (timeoutSearch) clearTimeout(timeoutSearch)
  timeoutSearch = setTimeout(() => {
    currentPage.value = 1
    loadData()
  }, 500)
})
//#endregion

//#region Methods

/**
 * Hàm gọi API lấy dữ liệu phân trang
 * createdby: kxtrien - 04.12.2025
 */
const isMock = false // bật/tắt mock

const loadData = async () => {
  try {
    isLoading.value = true
    const isMock = false // bật/tắt mock

    if (isMock) {
      // 👉 Fake data - Toàn bộ dữ liệu Hóa đơn điện tử (32 trường)
      const fakeTotal = 570

      const fakeData = Array.from({ length: itemsPerPage.value }, (_, i) => {
        const id = (currentPage.value - 1) * itemsPerPage.value + i + 1

        // Chuỗi ngẫu nhiên giả lập mã CQT
        const randomStr = Math.random().toString(36).substring(2, 10).toUpperCase()

        return {
          // 1. Nhóm định danh & Phân loại
          InvoiceId: id, // Đóng vai trò là khóa chính
          InvoiceType: 1,
          TemplateCode: '1',
          Series: 'C26TAA',
          InvoiceNumber: String(id).padStart(7, '0'),
          InvoiceDate: `2026-03-${String((id % 28) + 1).padStart(2, '0')}T10:00:00`,

          // 2. Nhóm thông tin người mua/bán
          SellerTaxCode: '0101243150',
          SellerName: 'Công ty Cổ phần MISA',
          BuyerTaxCode: `03123456${String(id).slice(-2)}`,
          BuyerName: `Nguyễn Văn ${id}`,
          BuyerLegalName: `Công ty TNHH Khách Hàng Số ${id}`,
          BuyerAddress: `Số ${id} Đường Cầu Giấy, Hà Nội`,
          BuyerEmail: `khachhang${id}@gmail.com`,
          BuyerPhone: `09876543${String(id % 100).padStart(2, '0')}`,

          // 3. Nhóm Tiền tệ & Thanh toán
          PaymentMethod: id % 2 === 0 ? 'TM/CK' : 'CK',
          CurrencyCode: 'VND',
          ExchangeRate: 1,

          // 4. Nhóm Tổng hợp Tài chính
          TotalBeforeTax: 1000000 * id,
          TotalDiscount: 0,
          TotalTaxAmount: 100000 * id,
          TotalAmount: 1100000 * id,
          TotalAmountInWords: 'Một triệu một trăm nghìn đồng chẵn',

          // 5. Nhóm Trạng thái & Cơ quan Thuế
          IssueStatus: 1,
          CqtCode: `A1D8F4C0B57F${randomStr}`,
          CqtStatus: id % 2 === 0 ? 2 : 1,
          SellerSignedDate: `2026-03-${String((id % 28) + 1).padStart(2, '0')}T10:05:00`,
          ReferenceInvoiceId: null,

          // 6. Nhóm Hệ thống & Audit
          Note: `Ghi chú cho hóa đơn xuất bán ${id}`,
          CreatedDate: `2026-03-${String((id % 28) + 1).padStart(2, '0')}T09:00:00`,
          CreatedBy: 'admin',
          ModifiedDate: `2026-03-${String((id % 28) + 1).padStart(2, '0')}T09:30:00`,
          ModifiedBy: 'admin',

          // Thuộc tính phụ trợ phục vụ UI toggle trạng thái trên template
          status: id % 2,
        }
      })

      totalRecords.value = fakeTotal
      displayData.value = fakeData

      return
    }

    // 👉 Code API thật giữ nguyên
    const params = {
      keyword: searchKeyword.value,
      pageIndex: currentPage.value,
      pageSize: itemsPerPage.value,
      sortOptions: activeSorts.value,
      filters: currentFilters.value,
    }

    const res = await invoiceApi.getFilterPaging(params)
    console.log(res)
    if (res && (res.code === 200 || res.code === 201)) {
      const payload = res.data || res
      totalRecords.value = payload.totalRecords ?? payload.totalRecords

      displayData.value = (payload.data || payload).map((item) => ({
        ...item,
        CreatedDate: item.CreatedDate,
        ModifiedDate: item.ModifiedDate,
        createdDate: formatDateString(item.CreatedDate),
        modifiedDate: formatDateString(item.ModifiedDate),
      }))
    }
  } catch (error) {
    console.error('Lỗi tải dữ liệu:', error)
  } finally {
    isLoading.value = false
  }
}
/**
 * Hàm xuất khẩu dữ liệu ra Excel
 * createdby: kxtrien - 07.12.2025
 */
const handleExport = async () => {
  try {
    // isLoading.value = true

    const params = {
      keyword: searchKeyword.value,
      pageIndex: currentPage.value,
      pageSize: totalRecords.value || 10000,
      sortOptions: activeSorts.value,
      filters: currentFilters.value,
    }
    const res = await invoiceApi.exportExcel(params)
    if (res) {
      const url = window.URL.createObjectURL(new Blob([res]))
      const link = document.createElement('a')
      link.href = url

      const fileName = `Danh_sach_hoa_don_${new Date().getTime()}.xlsx`
      link.setAttribute('download', fileName)

      document.body.appendChild(link)
      link.click()
      document.body.removeChild(link)
      window.URL.revokeObjectURL(url)
      toast.success('Xuất khẩu dữ liệu thành công!')
    }
  } catch (error) {
    console.error('Lỗi xuất khẩu:', error)
    toast.error('Có lỗi xảy ra khi xuất khẩu dữ liệu.')
  } finally {
    // isLoading.value = false
  }
}

// --- Filter Logic ---

/**
 * Hàm lấy label hiển thị cho cột (Dùng cho tag lọc)
 * @param prop
 * createdby: kxtrien - 01.12.2025
 */
const getColLabel = (prop) => {
  const col = shiftColumns.value.find((c) => c.prop === prop)
  return col ? col.label : prop
}

/**
 * Hàm xóa một bộ lọc cụ thể
 * @param key
 * createdby: kxtrien - 03.12.2025
 */
const removeFilter = (key) => {
  if (currentFilters.value[key]) {
    const newFilters = { ...currentFilters.value }
    delete newFilters[key]
    handleFilterChange(newFilters)
  }
}

/**
 * Hàm xóa tất cả bộ lọc
 * createdby: kxtrien - 03.12.2025
 */
const clearAllFilters = () => {
  currentFilters.value = {}
  handleFilterChange({})
}

/**
 * Hàm xử lý sự kiện thay đổi bộ lọc từ Table
 * @param newFilters
 * createdby: kxtrien - 03.12.2025
 */
const handleFilterChange = (newFilters) => {
  const cleanFilters = {}

  if (newFilters) {
    Object.entries(newFilters).forEach(([key, filter]) => {
      // Điều kiện giữ lại: Có giá trị HOẶC là toán tử đặc biệt (empty/not_empty)
      const hasValue = filter.value !== '' && filter.value !== null && filter.value !== undefined
      const isSpecialOp = ['empty', 'not_empty'].includes(filter.operator)

      if (hasValue || isSpecialOp) {
        const filterToSend = { ...filter }

        // Regex kiểm tra định dạng dd/mm/yyyy (Ví dụ: 08/12/2025)
        const datePattern = /^(\d{1,2})\/(\d{1,2})\/(\d{4})$/
        if (typeof filterToSend.value === 'string' && datePattern.test(filterToSend.value)) {
          const [_, day, month, year] = filterToSend.value.match(datePattern)

          // Chuyển đổi thành: yyyy-mm-dd (Ví dụ: 2025-12-08)
          filterToSend.value = `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`
        }
        cleanFilters[key] = filterToSend
      }
    })
  }

  currentFilters.value = cleanFilters

  currentPage.value = 1
  loadData()
  console.log('Filter: ', newFilters)
}

// --- Sort Logic ---

/**
 * Hàm xử lý sự kiện sắp xếp
 * @param colProp
 * @param type
 * createdby: kxtrien - 01.12.2025
 */
const handleSortChange = (colProp, type) => {
  const existingIndex = activeSorts.value.findIndex((s) => s.colProp === colProp)

  if (!type) {
    if (existingIndex !== -1) {
      activeSorts.value.splice(existingIndex, 1)
    }
  } else if (existingIndex == -1) {
    activeSorts.value.push({ colProp, type })
  } else {
    activeSorts.value[existingIndex].type = type
  }
  loadData()
  console.log('Sort: ', activeSorts.value)
}

// --- Selection Logic ---
/**
 * Hàm bỏ chọn tất cả dòng
 * createdby: kxtrien - 03.12.2025
 */
const handleUnselectRows = () => {
  selectedRowIds.value = []
}

/**
 * Khởi tạo kết nối SignalR để nhận thông báo cấp mã hóa đơn thời gian thực
 * createdby: kxtrien - 20.03.2024
 */
const startSignalR = async () => {
  const token = localStorage.getItem('token')
  connection = new signalR.HubConnectionBuilder()
    .withUrl('https://localhost:7248/notificationHub', {
      accessTokenFactory: () => token,
    })
    .withAutomaticReconnect()
    .build()

  connection.on('ReceiveInvoiceResult', (data) => {
    // Tìm hóa đơn trong danh sách đang hiển thị và cập nhật thông tin mới nhất
    const invoice = displayData.value.find(
      (item) => (item.InvoiceId || item.invoiceId) === data.InvoiceId,
    )

    if (invoice) {
      // Cập nhật mã CQT và trạng thái trực tiếp lên UI
      invoice.CqtCode = data.CqtCode
      invoice.cqtCode = data.CqtCode
      invoice.IssueStatus = data.Status
      invoice.issueStatus = data.Status
      invoice.CqtStatus = 3 // Chấp nhận

      toast.success(data.Message)
    }
  })

  try {
    await connection.start()
  } catch (err) {
    console.error('SignalR Error: ', err)
  }
}

// --- Popup Actions ---

/**
 * Hàm mở popup Thêm mới
 * createdby: kxtrien - 01.12.2025
 */
const handleOpenPopup = () => {
  router.push('/invoice/creation')
}

/**
 * Hàm mở popup Cập nhật
 * createdby: kxtrien - 01.12.2025
 */
const handleUpdateShift = (item) => {
  // Navigate to invoice creation page in edit mode
  if (item && item.InvoiceId) {
    router.push(`/invoice/creation/${item.InvoiceId}`)
  } else if (item && item.invoiceId) {
    router.push(`/invoice/creation/${item.invoiceId}`)
  } else {
    console.log('update: ', item)
  }
}

// Xem hóa đơn (mở render HTML trong new tab)
const handleViewInvoice = async (item) => {
  try {
    // Request PDF as binary blob so browser can render in PDF viewer
    const blob = await axiosClient.get(`/invoices/${item.invoiceId}/render`, {
      params: { format: 'pdf' },
      responseType: 'blob',
    })

    // axiosClient interceptor returns response.data, which should be the Blob
    if (!blob) {
      throw new Error('Không nhận được nội dung PDF từ server')
    }

    // If server returned JSON (error), try to parse and show message
    if (blob.type && blob.type.indexOf('application/json') !== -1) {
      const text = await blob.text()
      let parsed
      try {
        parsed = JSON.parse(text)
      } catch (_) {
        parsed = { message: text }
      }
      console.error('Server returned error when requesting PDF:', parsed)
      toast.error(parsed?.message || 'Lỗi khi tải PDF')
      return
    }

    const url = window.URL.createObjectURL(blob)
    const w = window.open(url, '_blank')
    if (!w) {
      // popup blocked - fallback to creating anchor
      const a = document.createElement('a')
      a.href = url
      a.target = '_blank'
      a.rel = 'noopener'
      a.click()
    }

    // revoke object URL after some time to release memory
    setTimeout(() => window.URL.revokeObjectURL(url), 1000 * 60)
  } catch (e) {
    console.error('Lỗi khi xem hóa đơn:', e)
    toast.error('Không thể xem hóa đơn. Kiểm tra log.')
  }
}

/**
 * Hàm đóng popup
 * createdby: kxtrien - 01.12.2025
 */
const handleClosePopup = () => {
  isOpenPopup.value = false
}

/**
 * Hàm xử lý submit Hóa đơn
 * createdby: kxtrien - 02.12.2025
 */
const validateMessage = ref('')
const handleSubmitShift = async (data, mode) => {
  let codeResponse = null
  try {
    const payload = processShiftPayload(data)
    console.log('payload: ', payload)

    let res
    if (data.InvoiceId) {
      res = await invoiceApi.update(data.InvoiceId, payload)
    } else {
      res = await invoiceApi.insert(payload)
    }

    if (res && (res.code === 200 || res.code === 201)) {
      // Thông báo thành công
      toast.success(data.InvoiceId ? 'Sửa thành công!' : 'Thêm mới thành công!')

      await loadData()

      // --- XỬ LÝ LOGIC ĐÓNG/MỞ FORM ---
      if (mode === 'saveAndAdd') {
        popupData.value = null
        popupCount.value++
      } else {
        handleClosePopup()
        if (!data.InvoiceId) currentPage.value = 1
      }
    } else {
      throw new Error(res?.message || 'Thất bại')
    }
  } catch (error) {
    codeResponse = error.response.data.code
    console.error('Lỗi submit:', error)
    validateMessage.value = error.response?.data?.message || error.message || 'Có lỗi xảy ra'
    isShowAlert.value = true
  }
}

/**
 * Hàm nhân bản dữ liệu
 * createdby: kxtrien - 08.12.2025
 */
const handleDuplicate = async (item) => {
  const { InvoiceId, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy, ...rest } = item
  const newItem = {
    ...rest,
    InvoiceNumber: rest.InvoiceNumber + '_COPY',
  }
  handleUpdateShift(newItem)
}

/**
 * Hàm cập nhật trạng thái (Sử dụng/Ngừng sử dụng) cho 1 dòng
 * createdby: kxtrien - 05.12.2025
 */
const handleToggleStatus = async (item) => {
  try {
    const newStatus = item.status === 1 ? 0 : 1
    await shiftApi.updateStatusMulti({ ids: [item.InvoiceId], status: newStatus })
    toast.success('Cập nhật trạng thái thành công')
    loadData()
  } catch (e) {
    console.error(e)
  }
}

/**
 * Hàm fotmat giá trị status
 * @param status
 * createdby: kxtrien - 01.12.2025
 */
const formatStatus = (status) => {
  if (status == 1) return 'Đang sử dụng'
  else if (status == 0) return 'Ngừng sử dụng'
  return 'Không xác định'
}

/**
 * Format hiển thị tên loại hóa đơn
 * createdby: kxtrien - 20.03.2024
 */
const formatInvoiceType = (type) => {
  switch (type) {
    case 1:
      return 'HĐ GTGT'
    case 2:
      return 'HĐ Bán hàng'
    case 0:
      return 'Hoá đơn giá trị gia tăng'
    default:
      return 'Khác'
  }
}

/**
 * Format hiển thị trạng thái phát hành hóa đơn
 * createdby: kxtrien - 20.03.2024
 */
const formatIssueStatus = (status) => {
  switch (status) {
    case 1:
      return 'Mới tạo'
    case 2:
      return 'Đã ký'
    case 3:
      return 'Đã phát hành'
    case 4:
      return 'Đã hủy'
    default:
      return 'Không xác định'
  }
}

/**
 * Lấy class CSS tương ứng cho trạng thái phát hành
 * createdby: kxtrien - 20.03.2024
 */
const getIssueStatusClass = (status) => {
  switch (status) {
    case 1:
      return 'is-new'
    case 2:
      return 'is-signed'
    case 3:
      return 'is-issued'
    case 4:
      return 'is-cancelled'
    default:
      return ''
  }
}

const handleBatchUpdateStatus = async (status) => {
  try {
    // API nhận vào: { ids: [...], status: 1 }
    const payload = {
      ids: selectedRowIds.value,
      status: status,
    }

    await shiftApi.updateStatusMulti(payload)

    toast.success('Cập nhật trạng thái thành công!')

    selectedRowIds.value = []
    await loadData()
  } catch (error) {
    console.error(error)
  }
}

/**
 * Hàm xử lý sự kiện click nút Xóa (icon thùng rác)
 * Lưu thông tin dòng cần xóa và mở Alert xác nhận
 * createdby: kxtrien - 08.12.2025
 */
const handleDelete = async (item) => {
  itemToDelete.value = item
  confirmInvoiceNumber.value = item.InvoiceNumber

  isMultipleDelete.value = false
  isShowAlert.value = true
}

/**
 * Hàm xử lý sự kiện click nút Xóa hàng loạt
 * Kiểm tra số lượng dòng chọn và mở Alert xác nhận
 * createdby: kxtrien - 08.12.2025
 */
const handleBatchDeleteShift = () => {
  if (selectedRowIds.value.length === 0) {
    toast.warning('Vui lòng chọn ít nhất một bản ghi để xóa.')
    return
  }

  isMultipleDelete.value = true
  isShowAlert.value = true
}

/**
 * Hàm thực thi xóa khi người dùng ấn nút Xóa trên Alert
 * createdby: kxtrien - 08.12.2025
 */
const handleConfirmDelete = async () => {
  try {
    isShowAlert.value = false

    if (isMultipleDelete.value) {
      await invoiceApi.deleteMulti(selectedRowIds.value)

      toast.success(`Đã xóa thành công ${selectedRowIds.value.length} hóa đơn!`)
      selectedRowIds.value = []
    } else {
      if (itemToDelete.value) {
        await invoiceApi.delete(itemToDelete.value.InvoiceId)
        toast.success('Xóa dữ liệu thành công!')
      }
    }

    currentPage.value = 1
    await loadData()
  } catch (error) {
    console.error('Lỗi xóa:', error)
    const msg = error.response?.data?.userMsg || 'Có lỗi xảy ra khi xóa dữ liệu.'
    toast.error(msg)
  } finally {
  }
}
/**
 * Hàm đóng Alert (Nút Hủy)
 * createdby: kxtrien - 08.12.2025
 */
const closeAlert = () => {
  isShowAlert.value = false
}
//#endregion

//#region: Life Cycle
onMounted(() => {
  loadData()
  startSignalR()
})

onUnmounted(() => {
  if (connection) connection.stop()
})
//#endregion
</script>
<template>
  <div class="shift">
    <div class="shift-top">
      <h3>Hoá đơn</h3>
      <MSButton @click="handleOpenPopup"><Plus class="icon" />Thêm</MSButton>
    </div>
    <div class="shift-container">
      <div class="condition-box">
        <div class="left-condition">
          <div class="input-container">
            <div class="icon-search icon"></div>
            <input type="text" placeholder="Tìm kiếm" v-model="searchKeyword" />
          </div>
          <div class="feature-batch" v-show="selectedRowIds.length > 0">
            <span
              >Đã chọn <b>{{ selectedRowIds.length }}</b></span
            >
            <span class="unselect" @click="handleUnselectRows">Bỏ chọn</span>
            <MSButton v-if="showBtnActive" class="button use" @click="handleBatchUpdateStatus(1)"
              ><div><CircleCheck size="14" />Sử dụng</div></MSButton
            >
            <MSButton v-if="showBtnInactive" class="button" @click="handleBatchUpdateStatus(0)"
              ><div><CircleOff size="14" />Ngừng sử dụng</div></MSButton
            >
            <MSButton class="button" @click="handleBatchDeleteShift"
              ><div><Trash2 size="14" />Xóa</div></MSButton
            >
          </div>
          <div class="filter-tags">
            <div v-for="(filter, key) in currentFilters" :key="key">
              <div v-if="filter.value" class="filter-tag">
                <span class="tag-label">{{ getColLabel(key) }}</span>
                <span class="tag-op">{{ opMap[filter.operator] || filter.operator }}</span>
                <span class="tag-value">{{ filter.value }}</span>
                <button class="tag-close" @click="removeFilter(key)">
                  <X :size="14" />
                </button>
              </div>
            </div>
          </div>
          <div v-if="hasActiveFilters" class="btn-clear-all" @click="clearAllFilters">Bỏ lọc</div>
        </div>
        <div class="right-buttons">
          <MSButton type="secondary" @click="loadData"><div class="icon-reload"></div></MSButton>
          <MSButton type="secondary" @click="handleExport"
            ><div class="icon-export"></div
          ></MSButton>
        </div>
      </div>
      <div class="shift-content">
        <MSTable
          :isLoading="isLoading"
          :columns="shiftColumns"
          :tableData="displayData"
          rowKey="invoiceId"
          :activeFilters="currentFilters"
          @filter-change="handleFilterChange"
          :activeSorts="activeSorts"
          @sort-change="handleSortChange"
          :selectedRowIds="selectedRowIds"
          @select-row="(ids) => (selectedRowIds = ids)"
          @update="handleUpdateShift"
          @duplicate="handleDuplicate"
          @toggle-status="handleToggleStatus"
          @delete="handleDelete"
          @view="handleViewInvoice"
        >
          <template #status="{ value }">
            <div v-if="value == '1'" class="status active">
              {{ formatStatus(value) }}
            </div>
            <div v-if="value == '0'" class="status inactive">
              {{ formatStatus(value) }}
            </div>
          </template>
          <template #invoiceType="{ value }">
            {{ formatInvoiceType(value) }}
          </template>
          <template #issueStatus="{ value }">
            <div :class="['status-publish', getIssueStatusClass(value)]">
              {{ formatIssueStatus(value) }}
            </div>
          </template>
          <template #date="{ value }">{{ formatDateString(value) }}</template>
        </MSTable>
      </div>
      <MSPagination
        v-model:pageSize="itemsPerPage"
        v-model:pageIndex="currentPage"
        :totalRecords="totalRecords"
        :pageSizeOptions="itemsPerPageOptions"
      />
    </div>
    <ShiftUpsertPopUp
      :key="popupCount"
      :popupData="popupData"
      v-if="isOpenPopup"
      @close="handleClosePopup"
      @submit="handleSubmitShift"
    ></ShiftUpsertPopUp>
    <MSAlert :is-show="isShowAlert" :title="validateMessage ? 'Cảnh báo!' : 'Xóa Hóa đơn'">
      <template #message>
        <p v-if="validateMessage">{{ validateMessage }}</p>
        <p v-else-if="isMultipleDelete">
          Các <b>Hóa đơn</b> sau khi bị xóa sẽ không thể khôi phục. Bạn có muốn tiếp tục xóa không?
        </p>
        <p v-else>
          Hóa đơn số <b>{{ confirmInvoiceNumber }}</b> sau khi bị xóa sẽ không thể khôi phục. Bạn có
          muốn tiếp tục xóa không?
        </p>
      </template>
      <template #footer>
        <MSButton v-if="!validateMessage" @click="closeAlert" type="secondary">Hủy</MSButton>
        <MSButton
          v-if="!validateMessage"
          @click="handleConfirmDelete"
          style="background-color: #dc2626; margin-left: 8px"
          >Xóa</MSButton
        >
        <MSButton v-if="validateMessage" @click="closeAlert">Đóng</MSButton>
      </template>
    </MSAlert>
  </div>
</template>

<style scoped>
.shift {
  display: flex;
  flex-direction: column;
  width: 100%;
  height: 100%;
  min-height: 0;
}
.shift-top {
  display: flex;
  align-items: start;
  justify-content: space-between;
  margin-bottom: 16px;
}
.shift-top h3 {
  font-weight: 700;
  color: #111827;
  font-size: 24px;
  height: 29.1429px;
}
.shift-top button {
}
.shift-top .icon {
  width: 16px;
}

/* Container */
.shift-container {
  flex: 1;
  border-radius: 8px;
  background: #ffffff;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  border: 1.5px solid #e8e8e8e8;
}

/* Top - Condition box */
.condition-box {
  min-height: 56px;
  padding: 8px 16px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.left-condition {
  display: flex;
  align-items: center;
  gap: 8px;
  flex: 1;
  overflow: hidden; /* Tránh tràn nếu nhiều tag quá */
  margin-right: 8px;
}

/* Style featurebatch */
.feature-batch {
  display: flex;
  align-items: center;
  margin-left: 8px;
  gap: 8px;
}
.feature-batch .unselect {
  margin: 0 8px;
  color: var(--color-primary);
  cursor: pointer;
}
.feature-batch .unselect:hover {
  text-decoration: underline;
}
.feature-batch .button {
  background-color: white;
  border: 1px solid #dc2626;
}
.feature-batch .button:hover {
  background-color: #fee2e2;
}

.feature-batch .button div {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 4px;
  color: rgb(220, 38, 38);
}
.feature-batch .use {
  border: 1px solid var(--color-primary);
}
.feature-batch .use:hover {
  background-color: #dcfce7;
}
.feature-batch .use div {
  color: var(--color-primary);
}

/* --- STYLE TAGS LỌC --- */
.filter-tags {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
  align-items: center;
  padding-bottom: 2px;
}
.filter-tags::-webkit-scrollbar {
  height: 0px;
  background: transparent;
}
.filter-tag {
  display: flex;
  align-items: center;
  gap: 8px;
  height: 24px;
  background-color: #f3f4f6;
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 13px;
  white-space: nowrap;
  color: #1f2937;
  border: 1px solid #e5e7eb;
}

.tag-op {
  color: #00a65a;
}
.right-buttons {
  display: flex;
  gap: 8px;
}

.tag-close {
  display: flex;
  align-items: center;
  justify-content: center;
  background: none;
  border: none;
  cursor: pointer;
  padding: 2px;
  margin-left: 4px;
  color: #9ca3af;
  border-radius: 50%;
  transition: all 0.2s;
}
.btn-clear-all {
  color: #f06666;
  cursor: pointer;
}
.btn-clear-all:hover {
  text-decoration: underline;
}

.input-container {
  position: relative;
  height: 32px;
  border: 1px solid #d1d5db;
  border-radius: 4px;
  min-width: 200px;
  max-width: 200px;
}
.input-container:hover {
  border-color: #9ca3af;
}
.input-container:focus-within {
  border-color: var(--color-primary);
}
.input-container .icon {
  position: absolute;
  top: 50%;
  left: 13px;
  transform: translateY(-50%);
}
.input-container input {
  width: 100%;
  height: 100%;
  border: none;
  outline: none;
  background: transparent;
  padding-left: 36px;
}

/* Main Content */
.shift-content {
  display: flex;
  flex-direction: column;
  overflow: hidden;
  min-height: 0;
  flex: 1;
  position: relative;
  width: 100%;
  min-height: 0;
}
.shift-content .status {
  padding: 5px 8px;
  line-height: 15px;
  font-weight: 400;
  border-radius: 999px;
}
.shift-content .status.active {
  background-color: #ebfef6;
  color: #009b71;
}
.shift-content .status.inactive {
  background-color: #fee2e2;
  color: #dc2626;
}

.shift-content .status-publish {
  padding: 4px 12px;
  border-radius: 12px;
  font-weight: 500;
  font-size: 12px;
  display: inline-block;
}
.shift-content .status-publish.is-new {
  background-color: #f3f4f6;
  color: #374151;
}
.shift-content .status-publish.is-signed {
  background-color: #e0f2fe;
  color: #0369a1;
}
.shift-content .status-publish.is-issued {
  background-color: #dcfce7;
  color: #15803d;
}
.shift-content .status-publish.is-cancelled {
  background-color: #fee2e2;
  color: #b91c1c;
}
</style>
