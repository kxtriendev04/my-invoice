<script setup>
import { ref, onMounted } from 'vue'
import { companyApi, userApi, companyUserApi } from '@/api/adminApi'
import MSButton from '@/components/ms-button/MSButton.vue'
import MSToast from '@/components/ms-toast/MSToast.vue'
import { Plus, Trash2, UserPlus, X } from 'lucide-vue-next'
import { useRoute } from 'vue-router'

const toastRef = ref(null)
const route = useRoute()

// Danh sách công ty và người dùng
const companies = ref([])
const users = ref([])
const companyUsers = ref([])

// Form chọn công ty và người dùng
const selectedCompany = ref('')
const selectedUser = ref('')
const selectedRole = ref(2) // 1: Admin, 2: Accountant, 3: Viewer

// Form thêm mới
const isShowForm = ref(false)
const isLoading = ref(false)
const isSaving = ref(false)

const roleOptions = [
  { label: 'Quản trị viên', value: 1 },
  { label: 'Kế toán', value: 2 },
  { label: 'Người xem', value: 3 },
]

const roleLabel = (roleId) => {
  const option = roleOptions.find((r) => r.value === roleId)
  return option ? option.label : 'Không xác định'
}

// Load danh sách công ty
const loadCompanies = async () => {
  try {
    const res = await companyApi.getFilterPaging({ pageIndex: 1, pageSize: 1000 })
    if (res && res.code === 200 && res.data) {
      companies.value = res.data.data || []
    }
  } catch (error) {
    console.error('Lỗi tải danh sách công ty:', error)
  }
}

// Load danh sách người dùng
const loadUsers = async () => {
  try {
    const res = await userApi.getFilterPaging({ pageIndex: 1, pageSize: 1000 })
    if (res && res.code === 200 && res.data) {
      users.value = res.data.data || []
    }
  } catch (error) {
    console.error('Lỗi tải danh sách người dùng:', error)
  }
}

// Load danh sách người dùng trong công ty
const loadCompanyUsers = async (companyId) => {
  if (!companyId) return
  try {
    isLoading.value = true
    const res = await companyUserApi.getUsersByCompany(companyId)
    if (res && res.code === 200 && res.data) {
      companyUsers.value = res.data.data || []
    }
  } catch (error) {
    console.error('Lỗi tải danh sách người dùng trong công ty:', error)
    toastRef.value?.showErrorToast('Không thể tải dữ liệu')
  } finally {
    isLoading.value = false
  }
}

// Xử lý đổi công ty
const handleCompanyChange = (companyId) => {
  selectedCompany.value = companyId
  loadCompanyUsers(companyId)
}

// Xử lý gán người dùng
const handleAssignUser = async () => {
  if (!selectedCompany.value) {
    toastRef.value?.showWarningToast('Vui lòng chọn công ty')
    return
  }
  if (!selectedUser.value) {
    toastRef.value?.showWarningToast('Vui lòng chọn người dùng')
    return
  }

  try {
    isSaving.value = true
    const data = {
      companyId: selectedCompany.value,
      userId: selectedUser.value,
      roleId: parseInt(selectedRole.value),
    }
    await companyUserApi.assignUserToCompany(data)
    toastRef.value?.showSuccessToast('Gán người dùng thành công')
    selectedUser.value = ''
    selectedRole.value = 2
    loadCompanyUsers(selectedCompany.value)
  } catch (error) {
    console.error('Lỗi gán người dùng:', error)
    toastRef.value?.showErrorToast('Không thể gán người dùng')
  } finally {
    isSaving.value = false
  }
}

// Xử lý xóa người dùng khỏi công ty
const handleRemoveUser = async (userId) => {
  if (!confirm('Bạn có chắc muốn xóa người dùng này khỏi công ty?')) return

  try {
    await companyUserApi.removeUserFromCompany({
      companyId: selectedCompany.value,
      userId,
    })
    toastRef.value?.showSuccessToast('Xóa thành công')
    loadCompanyUsers(selectedCompany.value)
  } catch (error) {
    console.error('Lỗi xóa:', error)
    toastRef.value?.showErrorToast('Không thể xóa')
  }
}

// Lấy tên công ty theo ID
const getCompanyName = (companyId) => {
  const company = companies.value.find((c) => c.companyId === companyId)
  return company ? company.companyName : 'Không xác định'
}

// Lấy tên người dùng theo ID
const getUserName = (userId) => {
  const user = users.value.find((u) => u.userId === userId)
  return user ? `${user.fullName} (${user.username})` : 'Không xác định'
}

onMounted(async () => {
  await Promise.all([loadCompanies(), loadUsers()])

  // Nếu có userId từ query string
  if (route.query.userId) {
    selectedUser.value = route.query.userId
  }

  // Nếu chỉ có một công ty thì chọn luôn
  if (companies.value.length === 1) {
    selectedCompany.value = companies.value[0].companyId
    loadCompanyUsers(selectedCompany.value)
  }
})
</script>

<template>
  <div class="assign-user">
    <div class="page-top">
      <h3>Gán tài khoản vào công ty</h3>
      <MSButton @click="$router.push('/admin/users')"> <X class="icon" />Quay lại </MSButton>
    </div>

    <div class="content-container">
      <!-- Chọn công ty -->
      <div class="card">
        <h4 class="card-title">Chọn công ty</h4>
        <div class="form-row">
          <div class="form-group">
            <label>Công ty</label>
            <select v-model="selectedCompany" @change="handleCompanyChange">
              <option value="">-- Chọn công ty --</option>
              <option v-for="c in companies" :key="c.companyId" :value="c.companyId">
                {{ c.companyName }} ({{ c.companyTaxCode }})
              </option>
            </select>
          </div>
        </div>
      </div>

      <!-- Form gán người dùng -->
      <div class="card" v-show="selectedCompany">
        <h4 class="card-title">
          <UserPlus :size="18" />
          Gán người dùng mới
        </h4>
        <div class="form-row">
          <div class="form-group">
            <label>Người dùng</label>
            <select v-model="selectedUser">
              <option value="">-- Chọn người dùng --</option>
              <option v-for="u in users" :key="u.userId" :value="u.userId">
                {{ u.fullName }} ({{ u.username }})
              </option>
            </select>
          </div>
          <div class="form-group">
            <label>Vai trò</label>
            <select v-model="selectedRole">
              <option v-for="r in roleOptions" :key="r.value" :value="r.value">
                {{ r.label }}
              </option>
            </select>
          </div>
          <div class="form-group form-actions">
            <MSButton @click="handleAssignUser" :disabled="isSaving || !selectedUser">
              <UserPlus :size="16" />
              {{ isSaving ? 'Đang xử lý...' : 'Gán người dùng' }}
            </MSButton>
          </div>
        </div>
      </div>

      <!-- Danh sách người dùng trong công ty -->
      <div class="card" v-show="selectedCompany">
        <h4 class="card-title">
          Người dùng trong công ty: <strong>{{ getCompanyName(selectedCompany) }}</strong>
        </h4>

        <div v-if="isLoading" class="loading">Đang tải...</div>
        <div v-else-if="companyUsers.length === 0" class="empty">
          Chưa có người dùng nào trong công ty này
        </div>
        <div v-else class="table-wrapper">
          <table class="table">
            <thead>
              <tr>
                <th style="width: 60px">STT</th>
                <th>Tên đăng nhập</th>
                <th>Họ và tên</th>
                <th>Email</th>
                <th>Vai trò</th>
                <th style="width: 100px">Thao tác</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, index) in companyUsers" :key="item.userId || index">
                <td>{{ index + 1 }}</td>
                <td>{{ item.username || 'N/A' }}</td>
                <td>{{ item.fullName || 'N/A' }}</td>
                <td>{{ item.email || 'N/A' }}</td>
                <td>{{ roleLabel(item.roleId) }}</td>
                <td>
                  <button class="btn-remove" @click="handleRemoveUser(item.userId)">
                    <Trash2 :size="14" /> Xóa
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <MSToast ref="toastRef" />
  </div>
</template>

<style scoped>
.assign-user {
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
.content-container {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.card {
  background: #ffffff;
  border-radius: 8px;
  padding: 20px 24px;
  border: 1.5px solid #e8e8e8;
}
.card-title {
  font-size: 16px;
  font-weight: 600;
  color: #374151;
  margin: 0 0 16px 0;
  display: flex;
  align-items: center;
  gap: 8px;
}
.form-row {
  display: flex;
  gap: 16px;
  flex-wrap: wrap;
  align-items: flex-end;
}
.form-group {
  flex: 1;
  min-width: 200px;
}
.form-group label {
  display: block;
  font-size: 14px;
  color: #6b7280;
  margin-bottom: 6px;
}
.form-group select {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 14px;
  background: white;
}
.form-group select:focus {
  outline: none;
  border-color: #2563eb;
}
.form-actions {
  flex: 0 0 auto;
  display: flex;
  align-items: flex-end;
}
.loading,
.empty {
  padding: 40px;
  text-align: center;
  color: #6b7280;
  font-size: 14px;
}
.table-wrapper {
  overflow-x: auto;
}
.table {
  width: 100%;
  border-collapse: collapse;
  font-size: 14px;
}
.table thead {
  background: #f9fafb;
}
.table th,
.table td {
  padding: 12px 16px;
  text-align: left;
  border-bottom: 1px solid #f3f4f6;
}
.table th {
  font-weight: 600;
  color: #374151;
  font-size: 13px;
  text-transform: uppercase;
}
.table tbody tr:hover {
  background: #f9fafb;
}
.btn-remove {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 6px 10px;
  background: white;
  border: 1px solid #fee2e2;
  border-radius: 4px;
  color: #dc2626;
  font-size: 13px;
  cursor: pointer;
  transition: all 0.2s;
}
.btn-remove:hover {
  background: #fee2e2;
}
</style>
