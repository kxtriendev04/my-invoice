<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { Plus, Trash2, Check, X } from 'lucide-vue-next'
import MSButton from '@/components/ms-button/MSButton.vue'
import MSPagination from '@/components/ms-pagination/MSPagination.vue'
import MSTable from '@/components/ms-table/MSTable.vue'
import MSAlert from '@/components/ms-alert/MSAlert.vue'
import MSToast from '@/components/ms-toast/MSToast.vue'
import { formatDateString } from '@/utils/format'
import invoiceTemplateApi from '@/api/invoiceTemplateApi'

const router = useRouter()
const toastRef = ref(null)

//#region State
const isLoading = ref(false)
const searchKeyword = ref('')
const displayData = ref([])
const totalRecords = ref(0)
const currentPage = ref(1)
const itemsPerPage = ref(20)
const itemsPerPageOptions = [10, 20, 30, 50, 100]

const selectedRowIds = ref([])
const activeSorts = ref([])
const currentFilters = ref({})

// Delete state
const isShowAlert = ref(false)
const itemToDelete = ref(null)
const isMultipleDelete = ref(false)

let timeoutSearch = null

const templateColumns = ref([
  { prop: 'templateCode', label: 'Mã mẫu', width: 150, align: 'left', filter: true, type: 'text' },
  {
    prop: 'templateName',
    label: 'Tên mẫu hóa đơn',
    width: 350,
    align: 'left',
    filter: true,
    type: 'text',
  },
  { prop: 'invSeries', label: 'Ký hiệu', width: 120, align: 'center', filter: true, type: 'text' },
  { prop: 'isDefault', label: 'Mặc định', width: 100, align: 'center', custom: 'isDefault' },
  {
    prop: 'createdDate',
    label: 'Ngày tạo',
    width: 150,
    align: 'center',
    filter: true,
    type: 'date',
    custom: 'date',
  },
])
//#endregion

//#region Methods
const loadData = async () => {
  try {
    isLoading.value = true
    const company = JSON.parse(localStorage.getItem('company') || '{}')

    const params = {
      keyword: searchKeyword.value,
      companyId: company.companyId,
      pageIndex: currentPage.value,
      pageSize: itemsPerPage.value,
      sortOptions: activeSorts.value,
      filters: currentFilters.value,
    }

    const res = await invoiceTemplateApi.getByCompany(params)
    if (res && (res.code === 200 || res.Code === 200) && res.data) {
      // Xử lý linh hoạt nếu API trả về mảng trực tiếp hoặc object phân trang
      const dataArray = Array.isArray(res.data) ? res.data : (res.data.data ?? res.data.Data ?? [])
      displayData.value = dataArray
      totalRecords.value = res.data.totalRecords ?? res.data.TotalRecords ?? dataArray.length
    } else {
      displayData.value = []
      totalRecords.value = 0
    }
  } catch (error) {
    console.error('Lỗi tải danh sách mẫu:', error)
    // Mock data để demo nếu API chưa sẵn sàng
    displayData.value = [
      {
        templateId: '1',
        templateCode: '1',
        templateName: 'Hóa đơn GTGT (Mẫu cơ bản)',
        invSeries: 'C26TAA',
        isDefault: true,
        status: 1,
        createdDate: new Date().toISOString(),
      },
      {
        templateId: '2',
        templateCode: '2',
        templateName: 'Hóa đơn bán hàng',
        invSeries: 'C26TBB',
        isDefault: false,
        status: 1,
        createdDate: new Date().toISOString(),
      },
    ]
    totalRecords.value = 2
  } finally {
    isLoading.value = false
  }
}

const handleOpenCreate = () => {
  router.push('/invoice/template/setup')
}

const handleUpdate = (item) => {
  router.push(`/invoice/template/${item.templateId}`)
}

const handleDelete = (item) => {
  itemToDelete.value = item
  isMultipleDelete.value = false
  isShowAlert.value = true
}

const handleConfirmDelete = async () => {
  try {
    if (isMultipleDelete.value) {
      await invoiceTemplateApi.deleteMulti(selectedRowIds.value)
      toastRef.value?.showSuccessToast('Xóa thành công các mẫu đã chọn')
    } else {
      await invoiceTemplateApi.delete(itemToDelete.value.templateId)
      toastRef.value?.showSuccessToast('Xóa mẫu hóa đơn thành công')
    }
    isShowAlert.value = false
    selectedRowIds.value = []
    loadData()
  } catch (error) {
    toastRef.value?.showErrorToast('Không thể xóa dữ liệu')
  }
}

const handleFilterChange = (newFilters) => {
  currentFilters.value = newFilters
  currentPage.value = 1
  loadData()
}

const handleSortChange = (colProp, type) => {
  activeSorts.value = [{ colProp, type }]
  loadData()
}

const handleBatchDelete = () => {
  isMultipleDelete.value = true
  isShowAlert.value = true
}
//#endregion

onMounted(() => {
  loadData()
})

watch([currentPage, itemsPerPage], () => loadData())

/**
 * Theo dõi từ khóa tìm kiếm (Debounce 500ms) để tự động load lại dữ liệu
 */
watch(searchKeyword, () => {
  if (timeoutSearch) clearTimeout(timeoutSearch)
  timeoutSearch = setTimeout(() => {
    currentPage.value = 1
    loadData()
  }, 500)
})
</script>

<template>
  <div class="template-list">
    <div class="template-top">
      <h3>Danh mục mẫu hóa đơn</h3>
      <MSButton @click="handleOpenCreate"><Plus class="icon" />Thêm mới</MSButton>
    </div>

    <div class="template-container">
      <div class="condition-box">
        <div class="left-condition">
          <div class="input-container">
            <div class="icon-search icon"></div>
            <input type="text" placeholder="Tìm kiếm theo mã, tên mẫu" v-model="searchKeyword" />
          </div>

          <div class="feature-batch" v-show="selectedRowIds.length > 0">
            <span
              >Đã chọn <b>{{ selectedRowIds.length }}</b></span
            >
            <span class="unselect" @click="selectedRowIds = []">Bỏ chọn</span>
            <MSButton class="button" @click="handleBatchDelete">
              <div><Trash2 size="14" />Xóa</div>
            </MSButton>
          </div>
        </div>

        <div class="right-buttons">
          <MSButton type="secondary" @click="loadData"><div class="icon-reload"></div></MSButton>
        </div>
      </div>

      <div class="template-content">
        <MSTable
          :isLoading="isLoading"
          :columns="templateColumns"
          :tableData="displayData"
          rowKey="templateId"
          :selectedRowIds="selectedRowIds"
          @select-row="(ids) => (selectedRowIds = ids)"
          @update="handleUpdate"
          @delete="handleDelete"
          @filter-change="handleFilterChange"
          @sort-change="handleSortChange"
        >
          <template #isDefault="{ value }">
            <Check v-if="value" class="text-green" size="18" />
            <X v-else="value" class="text-red" size="18" />
          </template>

          <template #status="{ value }">
            <div :class="['status', value === 1 ? 'active' : 'inactive']">
              {{ value === 1 ? 'Đang sử dụng' : 'Ngừng sử dụng' }}
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
    <MSAlert :is-show="isShowAlert" title="Xóa mẫu hóa đơn">
      <template #message>
        <p v-if="isMultipleDelete">Bạn có chắc chắn muốn xóa các mẫu hóa đơn đã chọn không?</p>
        <p v-else>
          Bạn có chắc chắn muốn xóa mẫu hóa đơn <b>{{ itemToDelete?.templateCode }}</b> không?
        </p>
      </template>
      <template #footer>
        <MSButton @click="isShowAlert = false" type="secondary">Hủy</MSButton>
        <MSButton @click="handleConfirmDelete" style="background-color: #dc2626; margin-left: 8px"
          >Xóa</MSButton
        >
      </template>
    </MSAlert>
  </div>
</template>

<style scoped>
.template-list {
  display: flex;
  flex-direction: column;
  width: 100%;
  height: 100%;
  min-height: 0;
}
.template-top {
  display: flex;
  align-items: start;
  justify-content: space-between;
  margin-bottom: 16px;
}
.template-top h3 {
  font-weight: 700;
  font-size: 24px;
}
.template-container {
  flex: 1;
  border-radius: 8px;
  background: #ffffff;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  border: 1.5px solid #e8e8e8;
}
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
}
.input-container {
  position: relative;
  height: 32px;
  border: 1px solid #d1d5db;
  border-radius: 4px;
  width: 250px;
}
.input-container .icon-search {
  position: absolute;
  top: 50%;
  left: 10px;
  transform: translateY(-50%);
}
.input-container input {
  width: 100%;
  height: 100%;
  border: none;
  padding-left: 35px;
  outline: none;
  font-size: 13px;
}
.template-content {
  flex: 1;
  position: relative;
}
.template-pagination {
  height: 44px;
  padding: 0 16px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-top: 1px solid #e5e7eb;
}
.main-pagination {
  display: flex;
  align-items: center;
  gap: 16px;
}
.btn-page {
  display: flex;
}
.btn-page button {
  background: transparent;
  border: none;
  cursor: pointer;
  padding: 4px;
}
.btn-page button:disabled {
  opacity: 0.3;
  cursor: not-allowed;
}
.status {
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 12px;
  width: fit-content;
  margin: 0 auto;
}
.status.active {
  background-color: #ebfef6;
  color: #009b71;
}
.status.inactive {
  background-color: #fee2e2;
  color: #dc2626;
}
.text-green {
  color: #10b981;
}
.text-red {
  color: red;
}
.feature-batch {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-left: 16px;
}
.unselect {
  color: #2563eb;
  cursor: pointer;
}
.feature-batch .button {
  border: 1px solid #dc2626;
  background: #fff;
  color: #dc2626;
}
</style>
