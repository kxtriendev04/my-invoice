<script setup>
import { ref, onMounted, computed, watch } from 'vue'
import { userApi } from '@/api/adminApi'
import MSButton from '@/components/ms-button/MSButton.vue'
import MSTable from '@/components/ms-table/MSTable.vue'
import MSToast from '@/components/ms-toast/MSToast.vue'
import { Plus, Search, Edit, Trash2 } from 'lucide-vue-next'
import { formatDateString } from '@/utils/format'

const userColumns = ref([
  { label: 'Tên đăng nhập', prop: 'username', sortable: true, filterable: true, width: '150px' },
  { label: 'Họ và tên', prop: 'fullName', sortable: true, filterable: true },
  { label: 'Email', prop: 'email', sortable: true, filterable: true },
  { label: 'Trạng thái', prop: 'isActive', sortable: true, filterable: true, width: '120px' },
  { label: 'Ngày tạo', prop: 'createdDate', sortable: true, width: '150px' },
])

const users = ref([])
const totalRecords = ref(0)
const currentPage = ref(1)
const itemsPerPage = ref(20)
const searchKeyword = ref('')
const currentFilters = ref({})
const activeSorts = ref([])
const isLoading = ref(false)
const selectedRowIds = ref([])
const toastRef = ref(null)

const totalPages = computed(() => {
  if (itemsPerPage.value === 0) return 0
  return Math.ceil(totalRecords.value / itemsPerPage.value)
})

const recordRange = computed(() => {
  if (totalRecords.value === 0) return '0 - 0'
  const from = (currentPage.value - 1) * itemsPerPage.value + 1
  const to = Math.min(currentPage.value * itemsPerPage.value, totalRecords.value)
  return `${from} - ${to}`
})

const statusLabel = (isActive) => {
  return isActive ? 'Hoạt động' : 'Khóa'
}

const loadData = async () => {
  try {
    isLoading.value = true
    const params = {
      keyword: searchKeyword.value,
      pageIndex: currentPage.value,
      pageSize: itemsPerPage.value,
      sortOptions: activeSorts.value,
      filters: currentFilters.value,
    }
    const res = await userApi.getFilterPaging(params)
    if (res && res.code === 200 && res.data) {
      totalRecords.value = res.data.totalRecords
      users.value = res.data.data || []
    }
  } catch (error) {
    console.error('Lỗi tải dữ liệu:', error)
    toastRef.value?.showErrorToast('Không thể tải dữ liệu người dùng')
  } finally {
    isLoading.value = false
  }
}

const handleFilterChange = (newFilters) => {
  currentFilters.value = newFilters || {}
  currentPage.value = 1
  loadData()
}

const handleSortChange = (colProp, type) => {
  const existingIndex = activeSorts.value.findIndex((s) => s.colProp === colProp)
  if (!type) {
    if (existingIndex !== -1) activeSorts.value.splice(existingIndex, 1)
  } else if (existingIndex === -1) {
    activeSorts.value.push({ colProp, type })
  } else {
    activeSorts.value[existingIndex].type = type
  }
  loadData()
}

watch([currentPage, itemsPerPage], () => loadData())

let timeoutSearch = null
watch(searchKeyword, () => {
  if (timeoutSearch) clearTimeout(timeoutSearch)
  timeoutSearch = setTimeout(() => {
    currentPage.value = 1
    loadData()
  }, 500)
})

const handleDelete = async (row) => {
  if (confirm(`Bạn có chắc muốn xóa tài khoản "${row.fullName}"?`)) {
    try {
      await userApi.delete(row.userId)
      toastRef.value?.showSuccessToast('Xóa tài khoản thành công')
      loadData()
    } catch (error) {
      console.error('Lỗi xóa:', error)
      toastRef.value?.showErrorToast('Không thể xóa tài khoản')
    }
  }
}

onMounted(() => loadData())
</script>

<template>
  <div class="user-list">
    <div class="page-top">
      <h3>Quản lý tài khoản</h3>
      <MSButton @click="$router.push('/admin/user/create')">
        <Plus class="icon" />Thêm tài khoản
      </MSButton>
    </div>

    <div class="list-container">
      <div class="condition-box">
        <div class="left-condition">
          <div class="input-container">
            <div class="icon-search"><Search :size="16" /></div>
            <input type="text" placeholder="Tìm kiếm..." v-model="searchKeyword" />
          </div>
        </div>
        <div class="right-buttons">
          <MSButton type="secondary" @click="loadData">Làm mới</MSButton>
        </div>
      </div>

      <div class="table-content">
        <MSTable
          :isLoading="isLoading"
          :columns="userColumns"
          :tableData="users"
          rowKey="userId"
          :activeFilters="currentFilters"
          @filter-change="handleFilterChange"
          :activeSorts="activeSorts"
          @sort-change="handleSortChange"
          :selectedRowIds="selectedRowIds"
          @select-row="(ids) => (selectedRowIds = ids)"
        >
          <template #isActive="{ value }">
            <div v-if="value" class="status active">{{ statusLabel(value) }}</div>
            <div v-else class="status inactive">{{ statusLabel(value) }}</div>
          </template>
          <template #createdDate="{ value }">{{ formatDateString(value) }}</template>
          <template #actions="{ row }">
            <div class="action-buttons">
              <button class="btn-action" @click="$router.push(`/admin/user/${row.userId}`)">
                <Edit :size="14" />
              </button>
              <button class="btn-action delete" @click="handleDelete(row)">
                <Trash2 :size="14" />
              </button>
              <button
                class="btn-action assign"
                @click="$router.push(`/admin/assign-user?userId=${row.userId}`)"
              >
                Gán công ty
              </button>
            </div>
          </template>
        </MSTable>
      </div>

      <div class="pagination">
        <div class="pagination-count">
          Tổng số:
          <h6>{{ totalRecords }}</h6>
        </div>
        <div class="pagination-main">
          <span>Số dòng/trang</span>
          <select v-model.number="itemsPerPage">
            <option :value="10">10</option>
            <option :value="20">20</option>
            <option :value="50">50</option>
            <option :value="100">100</option>
          </select>
          <p>{{ recordRange }}</p>
          <div class="btn-page">
            <button @click="currentPage = 1" :disabled="currentPage === 1">Đầu</button>
            <button @click="currentPage--" :disabled="currentPage === 1">Trước</button>
            <button @click="currentPage++" :disabled="currentPage === totalPages">Sau</button>
            <button @click="currentPage = totalPages" :disabled="currentPage === totalPages">
              Cuối
            </button>
          </div>
        </div>
      </div>
    </div>

    <MSToast ref="toastRef" />
  </div>
</template>

<style scoped>
.user-list {
  display: flex;
  flex-direction: column;
  width: 100%;
  height: 100%;
  min-height: 0;
}
.page-top {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}
.page-top h3 {
  font-weight: 700;
  color: #111827;
  font-size: 24px;
}
.page-top .icon {
  width: 16px;
}
.list-container {
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
  border-bottom: 1px solid #f3f4f6;
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
  min-width: 200px;
}
.input-container .icon-search {
  position: absolute;
  top: 50%;
  left: 10px;
  transform: translateY(-50%);
  color: #9ca3af;
}
.input-container input {
  width: 100%;
  height: 100%;
  border: none;
  outline: none;
  background: transparent;
  padding-left: 36px;
  font-size: 14px;
}
.right-buttons {
  display: flex;
  gap: 8px;
}
.table-content {
  position: relative;
  flex: 1;
  overflow: auto;
  min-height: 0;
}
.status {
  padding: 4px 10px;
  border-radius: 999px;
  font-size: 13px;
  text-align: center;
}
.status.active {
  background-color: #ebfef6;
  color: #009b71;
}
.status.inactive {
  background-color: #fee2e2;
  color: #dc2626;
}
.action-buttons {
  display: flex;
  gap: 4px;
  justify-content: center;
}
.btn-action {
  background: none;
  border: 1px solid #e5e7eb;
  border-radius: 4px;
  padding: 4px 8px;
  cursor: pointer;
  color: #6b7280;
  transition: all 0.2s;
  font-size: 13px;
}
.btn-action:hover {
  background-color: #f3f4f6;
  color: #2563eb;
}
.btn-action.delete:hover {
  color: #dc2626;
}
.btn-action.assign:hover {
  color: #059669;
}
.pagination {
  height: 44px;
  background: #ffffff;
  padding: 8px 16px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-top: 1px solid #f3f4f6;
}
.pagination-count {
  display: flex;
  gap: 4px;
}
.pagination-count h6 {
  font-weight: 700;
}
.pagination-main {
  display: flex;
  align-items: center;
  gap: 12px;
}
.pagination-main select {
  padding: 4px 8px;
  border: 1px solid #d1d5db;
  border-radius: 4px;
}
.btn-page {
  display: flex;
  gap: 4px;
}
.btn-page button {
  padding: 4px 12px;
  border: 1px solid #d1d5db;
  border-radius: 4px;
  background: white;
  cursor: pointer;
}
.btn-page button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
</style>
