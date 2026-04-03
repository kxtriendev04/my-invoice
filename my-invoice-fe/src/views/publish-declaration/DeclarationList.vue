<script setup>
import MSButton from '@/components/ms-button/MSButton.vue'
import MSAlert from '@/components/ms-alert/MSAlert.vue'
import MSPagination from '@/components/ms-pagination/MSPagination.vue'
import MSTable from '@/components/ms-table/MSTable.vue'
import { formatDateString } from '@/utils/format'
import { CircleCheck, CircleOff, Plus, Trash2, X } from 'lucide-vue-next'
import { computed, onMounted, ref, watch, watchEffect } from 'vue'
import { useRouter } from 'vue-router'
import invoiceRegistrationApi from '@/api/invoiceRegistrationApi'
import { useToastStore } from '@/stores/useToastStore'

//#region Constants
const router = useRouter()
const opMap = {
  eq: 'Bằng',
  neq: 'Khác',
  gt: 'Lớn hơn',
  gte: 'Lớn hơn hoặc bằng',
  lt: 'Nhỏ hơn',
  lte: 'Nhỏ hơn hoặc bằng',
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
const toast = useToastStore()
const isLoading = ref(false)

// Pagination State
const itemsPerPage = ref(20)
const itemsPerPageOptions = ref([10, 20, 30, 50, 100])
const currentPage = ref(1)
const totalRecords = ref(0)

// Search State
const searchKeyword = ref('')

// Table Data State
const declarationColumns = ref([
  {
    prop: 'registrationNo',
    label: 'Số tờ khai',
    width: 150,
    align: 'left',
    filter: true,
    type: 'text',
  },
  {
    prop: 'createdDate',
    label: 'Ngày lập',
    width: 150,
    align: 'center',
    filter: true,
    type: 'date',
    custom: 'date',
  },
  {
    prop: 'transType',
    label: 'Loại đăng ký',
    width: 150,
    align: 'center',
    filter: true,
    type: 'select',
  },
  {
    prop: 'taxpayerName',
    label: 'Tên người nộp thuế',
    width: 250,
    align: 'left',
    filter: true,
    type: 'text',
  },
  { prop: 'taxCode', label: 'Mã số thuế', width: 150, align: 'center', filter: true, type: 'text' },
  {
    prop: 'contactPhone',
    label: 'SĐT liên hệ',
    width: 150,
    align: 'center',
    filter: true,
    type: 'text',
  },
  { prop: 'contactEmail', label: 'Email', width: 200, align: 'left', filter: true, type: 'text' },
  {
    prop: 'contactAddress',
    label: 'Địa chỉ liên hệ',
    width: 300,
    align: 'left',
    filter: true,
    type: 'text',
  },
  {
    prop: 'representativeName',
    label: 'Người đại diện',
    width: 200,
    align: 'left',
    filter: true,
    type: 'text',
  },
  {
    prop: 'status',
    label: 'Trạng thái',
    width: 150,
    align: 'center',
    filter: true,
    type: 'select',
    custom: 'status',
  },
  {
    prop: 'invoiceAppType',
    label: 'Loại HĐĐT',
    width: 150,
    align: 'center',
    filter: true,
    type: 'select',
  },
])
const displayData = ref([])

// Feature State (Filter, Sort, Select)
const currentFilters = ref({})
const activeSorts = ref([])
const selectedRowIds = ref([])

// Delete State
const isShowAlert = ref(false)
const itemToDelete = ref(null)
const isMultipleDelete = ref(false)
const confirmRegistrationNo = ref('')

let timeoutSearch = null
//#endregion

//#region Computed
/**
 * Kiểm tra xem có đang áp dụng bộ lọc nào không
 */
const hasActiveFilters = computed(() => {
  const filters = Object.values(currentFilters.value || {})
  return filters.some((f) => {
    return f.value !== '' && f.value != null
  })
})

/**
 * Lấy ra danh sách đầy đủ các object đang được chọn
 */
const selectedItems = computed(() => {
  return displayData.value.filter((item) =>
    selectedRowIds.value.includes(item.registrationId || item.RegistrationId),
  )
})

/**
 * Kiểm tra xem có item nào đang ở trạng thái 'Ngừng sử dụng' (0) để hiện nút 'Sử dụng' không
 */
const showBtnActive = computed(() => {
  return selectedItems.value.some((item) => item.Status === 0)
})

/**
 * Kiểm tra xem có item nào đang ở trạng thái 'Đang sử dụng' (1) để hiện nút 'Ngừng sử dụng' không
 */
const showBtnInactive = computed(() => {
  return selectedItems.value.some((item) => item.Status === 1)
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
 */
watch([currentPage, itemsPerPage], () => {
  loadData()
})

/**
 * Theo dõi từ khóa tìm kiếm (Debounce 500ms)
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
 */
const loadData = async () => {
  try {
    isLoading.value = true
    const company = JSON.parse(localStorage.getItem('company') || '{}')

    const params = {
      keyword: searchKeyword.value,
      companyId: company.companyId, // Thêm CompanyId để Backend xác thực dữ liệu theo đơn vị
      pageIndex: currentPage.value,
      pageSize: itemsPerPage.value,
      sortOptions: activeSorts.value,
      filters: currentFilters.value,
    }
    console.log('params', params)

    const res = await invoiceRegistrationApi.getByCompany(params)
    console.log('API response:', res)

    const dataRes = res?.data || res
    if (dataRes) {
      const dataArray = dataRes.data || dataRes.Data || (Array.isArray(dataRes) ? dataRes : [])
      totalRecords.value = dataRes.totalRecords || dataRes.TotalRecords || dataArray.length
      displayData.value = dataArray
    }
  } catch (error) {
    console.error('Lỗi tải dữ liệu:', error)
    displayData.value = []
    totalRecords.value = 0
  } finally {
    isLoading.value = false
  }
}

/**
 * Hàm xuất khẩu dữ liệu ra Excel
 */
const handleExport = async () => {
  try {
    const params = {
      keyword: searchKeyword.value,
      pageIndex: currentPage.value,
      pageSize: totalRecords.value || 10000,
      sortOptions: activeSorts.value,
      filters: currentFilters.value,
    }
    const res = await invoiceRegistrationApi.exportExcel(params)
    if (res) {
      const url = window.URL.createObjectURL(new Blob([res]))
      const link = document.createElement('a')
      link.href = url

      const fileName = `Danh_sach_to_khai_${new Date().getTime()}.xlsx`
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
  }
}

// --- Filter Logic ---

const getColLabel = (prop) => {
  const col = declarationColumns.value.find((c) => c.prop === prop)
  return col ? col.label : prop
}

const removeFilter = (key) => {
  if (currentFilters.value[key]) {
    const newFilters = { ...currentFilters.value }
    delete newFilters[key]
    handleFilterChange(newFilters)
  }
}

const clearAllFilters = () => {
  currentFilters.value = {}
  handleFilterChange({})
}

const handleFilterChange = (newFilters) => {
  const cleanFilters = {}

  if (newFilters) {
    Object.entries(newFilters).forEach(([key, filter]) => {
      const hasValue = filter.value !== '' && filter.value !== null && filter.value !== undefined
      const isSpecialOp = ['empty', 'not_empty'].includes(filter.operator)

      if (hasValue || isSpecialOp) {
        const filterToSend = { ...filter }

        const datePattern = /^(\d{1,2})\/(\d{1,2})\/(\d{4})$/
        if (typeof filterToSend.value === 'string' && datePattern.test(filterToSend.value)) {
          const [_, day, month, year] = filterToSend.value.match(datePattern)
          filterToSend.value = `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`
        }
        cleanFilters[key] = filterToSend
      }
    })
  }

  currentFilters.value = cleanFilters
  currentPage.value = 1
  loadData()
}

// --- Sort Logic ---

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
}

// --- Selection Logic ---

const handleUnselectRows = () => {
  selectedRowIds.value = []
}

// --- Popup Actions ---

const handleOpenPopup = () => {
  router.push('/invoice/declaration/create')
}

/**
 * Hàm format trạng thái
 */
const formatStatus = (status) => {
  switch (status) {
    case 0:
      return 'Chờ duyệt'
    case 1:
      return 'Đã duyệt'
    case 2:
      return 'Đang xử lý'
    case 3:
      return 'Từ chối'
    case 4:
      return 'Đã gửi'
    default:
      return 'Không xác định'
  }
}

/**
 * Hàm format loại đăng ký
 */
const formatTransType = (type) => {
  switch (type) {
    case 1:
      return 'Đăng ký mới'
    case 2:
      return 'Thay đổi thông tin'
    default:
      return 'Không xác định'
  }
}

/**
 * Hàm format loại HĐĐT
 */
const formatInvoiceAppType = (type) => {
  switch (type) {
    case 1:
      return 'Có mã CQT'
    case 2:
      return 'Không mã CQT'
    case 3:
      return 'Từ máy tính tiền'
    default:
      return 'Không xác định'
  }
}

// --- Batch Update Status ---

const handleBatchUpdateStatus = async (status) => {
  try {
    const payload = {
      ids: selectedRowIds.value,
      status: status,
    }

    await invoiceRegistrationApi.updateStatusMulti(payload)

    toast.success('Cập nhật trạng thái thành công!')

    selectedRowIds.value = []
    await loadData()
  } catch (error) {
    console.error(error)
    toast.error('Có lỗi xảy ra khi cập nhật trạng thái.')
  }
}

// --- Delete Actions ---

const handleDelete = async (item) => {
  itemToDelete.value = item
  confirmRegistrationNo.value = item.registrationNo

  isMultipleDelete.value = false
  isShowAlert.value = true
}

const handleBatchDelete = () => {
  if (selectedRowIds.value.length === 0) {
    toast.warning('Vui lòng chọn ít nhất một bản ghi để xóa.')
    return
  }

  isMultipleDelete.value = true
  isShowAlert.value = true
}

const handleConfirmDelete = async () => {
  try {
    isShowAlert.value = false

    if (isMultipleDelete.value) {
      await invoiceRegistrationApi.deleteMulti(selectedRowIds.value)

      toast.success(`Đã xóa thành công ${selectedRowIds.value.length} tờ khai!`)
      selectedRowIds.value = []
    } else {
      if (itemToDelete.value) {
        const id = itemToDelete.value.registrationId || itemToDelete.value.RegistrationId
        await invoiceRegistrationApi.delete(id)
        toast.success('Xóa dữ liệu thành công!')
      }
    }

    currentPage.value = 1
    await loadData()
  } catch (error) {
    console.error('Lỗi xóa:', error)
    const msg = error.response?.data?.userMsg || 'Có lỗi xảy ra khi xóa dữ liệu.'
    toast.error(msg)
  }
}

const closeAlert = () => {
  isShowAlert.value = false
}

// --- View Detail ---

const handleViewDetail = (item) => {
  const id = item.registrationId || item.RegistrationId
  router.push(`/invoice/declaration/${id}`)
}

// Handle edit action from table (open edit route)
const handleEdit = (item) => {
  const id = item?.registrationId || item?.RegistrationId
  if (id) router.push(`/invoice/declaration/${id}`)
}
//#endregion

//#region: Life Cycle
onMounted(() => {
  loadData()
})
//#endregion
</script>

<template>
  <div class="declaration">
    <div class="declaration-top">
      <h3>Danh sách tờ khai đăng ký hóa đơn</h3>
      <MSButton @click="handleOpenPopup"><Plus class="icon" />Thêm mới</MSButton>
    </div>
    <div class="declaration-container">
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
            <MSButton class="button" @click="handleBatchDelete"
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
      <div class="declaration-content">
        <MSTable
          :isLoading="isLoading"
          :columns="declarationColumns"
          :tableData="displayData"
          rowKey="registrationId"
          :activeFilters="currentFilters"
          @filter-change="handleFilterChange"
          :activeSorts="activeSorts"
          @sort-change="handleSortChange"
          :selectedRowIds="selectedRowIds"
          @select-row="(ids) => (selectedRowIds = ids)"
          @view="handleViewDetail"
          @update="handleEdit"
          @delete="handleDelete"
        >
          <template #status="{ value }">
            <div v-if="value == 1" class="status active">
              {{ formatStatus(value) }}
            </div>
            <div v-else-if="value == 2" class="status sent">
              {{ formatStatus(value) }}
            </div>
            <div v-else-if="value == 0" class="status pending">
              {{ formatStatus(value) }}
            </div>
            <div v-else-if="value == 3" class="status rejected">
              {{ formatStatus(value) }}
            </div>
            <div v-else class="status unknown">
              {{ formatStatus(value) }}
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
    <MSToast ref="toastRef" />
    <MSAlert :is-show="isShowAlert" :title="'Xóa tờ khai đăng ký'">
      <template #message>
        <p v-if="isMultipleDelete">
          Các <b>Tờ khai đăng ký</b> sau khi bị xóa sẽ không thể khôi phục. Bạn có muốn tiếp tục xóa
          không?
        </p>
        <p v-else>
          Tờ khai số <b>{{ confirmRegistrationNo }}</b> sau khi bị xóa sẽ không thể khôi phục. Bạn
          có muốn tiếp tục xóa không?
        </p>
      </template>
      <template #footer>
        <MSButton @click="closeAlert" type="secondary">Hủy</MSButton>
        <MSButton @click="handleConfirmDelete" style="background-color: #dc2626; margin-left: 8px"
          >Xóa</MSButton
        >
      </template>
    </MSAlert>
  </div>
</template>

<style scoped>
.declaration {
  display: flex;
  flex-direction: column;
  width: 100%;
  height: 100%;
  min-height: 0;
}
.declaration-top {
  display: flex;
  align-items: start;
  justify-content: space-between;
  margin-bottom: 16px;
}
.declaration-top h3 {
  font-weight: 700;
  color: #111827;
  font-size: 24px;
  height: 29.1429px;
}
.declaration-top .icon {
  width: 16px;
}

/* Container */
.declaration-container {
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
  overflow: hidden;
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
.declaration-content {
  display: flex;
  flex-direction: column;
  overflow: hidden;
  min-height: 0;
  flex: 1;
  position: relative;
  width: 100%;
  min-height: 0;
}
.declaration-content .status {
  padding: 5px 8px;
  line-height: 15px;
  font-weight: 400;
  border-radius: 999px;
}
.declaration-content .status.active {
  background-color: #ebfef6;
  color: #009b71;
}
.declaration-content .status.sent {
  background-color: #dbeafe;
  color: #2563eb;
}
.declaration-content .status.pending {
  background-color: #fef3c7;
  color: #b45309;
}
.declaration-content .status.rejected {
  background-color: #fee2e2;
  color: #dc2626;
}
.declaration-content .status.unknown {
  background-color: #f3f4f6;
  color: #6b7280;
}
</style>
