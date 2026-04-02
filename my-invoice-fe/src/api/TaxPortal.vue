<template>
  <div class="tax-portal-container">
    <header class="tax-header">
      <div class="logo-section">
        <img
          src="https://phucgia.com.vn/wp-content/uploads/2017/12/Logo_Tong_Cuc_Thue.png"
          width="100"
        />
        <div>
          <h1>TỔNG CỤC THUẾ</h1>
          <p>HỆ THỐNG QUẢN LÝ HÓA ĐƠN ĐIỆN TỬ (SIMULATION)</p>
        </div>
      </div>
      <div class="user-info">Cán bộ thuế: <b>Admin_System</b></div>
    </header>

    <main class="tax-content">
      <div class="tax-tabs">
        <button
          :class="['tab-item', { active: activeTab === 'registration' }]"
          @click="activeTab = 'registration'"
        >
          Tờ khai đăng ký
        </button>
        <button
          :class="['tab-item', { active: activeTab === 'invoice' }]"
          @click="activeTab = 'invoice'"
        >
          Hóa đơn chờ cấp mã
        </button>
      </div>

      <div class="card">
        <div class="card-header">
          <h2>Danh sách {{ activeTab === 'registration' ? 'tờ khai' : 'hóa đơn' }} chờ xử lý</h2>
          <button class="btn-refresh" @click="fetchData">Làm mới danh sách</button>
        </div>

        <table class="tax-table" v-if="activeTab === 'registration'">
          <thead>
            <tr>
              <th>STT</th>
              <th>Số tờ khai</th>
              <th>Mã số thuế</th>
              <th>Tên đơn vị</th>
              <th>Ngày gửi</th>
              <th class="text-center">Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="pendingList.length === 0">
              <td colspan="6" class="text-center py-20">
                Hiện không có tờ khai nào đang chờ duyệt.
              </td>
            </tr>
            <tr v-for="(item, index) in pendingList" :key="item.registrationId">
              <td>{{ index + 1 }}</td>
              <td class="bold">{{ item.registrationNo }}</td>
              <td>{{ item.taxCode }}</td>
              <td>{{ item.taxpayerName }}</td>
              <td>{{ new Date(item.createdDate).toLocaleString() }}</td>
              <td class="text-center">
                <button class="btn-approve" @click="approve(item.registrationId)">Phê duyệt</button>
                <button class="btn-reject ml-8">Từ chối</button>
              </td>
            </tr>
          </tbody>
        </table>

        <table class="tax-table" v-else>
          <thead>
            <tr>
              <th>STT</th>
              <th>Mẫu số - Ký hiệu</th>
              <th>Số hóa đơn</th>
              <th>Mã số thuế</th>
              <th>Tổng tiền</th>
              <th class="text-center">Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="pendingInvoices.length === 0">
              <td colspan="6" class="text-center py-20">Không có hóa đơn nào chờ cấp mã.</td>
            </tr>
            <tr v-for="(item, index) in pendingInvoices" :key="item.invoiceId">
              <td>{{ index + 1 }}</td>
              <td class="bold">{{ item.series }}</td>
              <td>{{ item.invoiceNumber || 'Chưa cấp số' }}</td>
              <td>{{ item.taxCode }}</td>
              <td class="text-right bold">
                {{ new Intl.NumberFormat('vi-VN').format(item.totalAmount) }}đ
              </td>
              <td class="text-center">
                <button class="btn-approve" @click="approveInv(item.invoiceId)">Cấp mã CQT</button>
                <button class="btn-reject ml-8">Từ chối</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import taxPortalApi from '@/api/taxPortalApi'

const activeTab = ref('registration')
const pendingList = ref([])
const pendingInvoices = ref([])

const fetchData = async () => {
  if (activeTab.value === 'registration') {
    try {
      const res = await taxPortalApi.getPendingRegistrations()
      pendingList.value = res.data || res
    } catch (error) {
      console.error(error)
    }
  } else {
    try {
      const res = await taxPortalApi.getPendingInvoices()
      pendingInvoices.value = res.data || res
    } catch (error) {
      console.error(error)
    }
  }
}

const approve = async (id) => {
  if (confirm('Bạn có chắc chắn muốn phê duyệt tờ khai này không?')) {
    try {
      await taxPortalApi.approveRegistration(id)
      alert('Phê duyệt thành công! Thông báo đã được gửi về phần mềm.')
      fetchData()
    } catch (error) {
      alert('Có lỗi xảy ra.')
    }
  }
}

const approveInv = async (id) => {
  if (confirm('Bạn có chắc chắn muốn cấp mã cho hóa đơn này?')) {
    try {
      await taxPortalApi.approveInvoice(id)
      alert('Đã cấp mã CQT thành công!')
      fetchData()
    } catch (error) {
      alert('Có lỗi xảy ra.')
    }
  }
}

watch(activeTab, fetchData)
onMounted(fetchData)
</script>

<style scoped>
/* Thêm Style cho Tabs */
.tax-tabs {
  display: flex;
  gap: 4px;
  margin-bottom: 0px;
}
.tab-item {
  padding: 12px 24px;
  background: #e0e0e0;
  border: none;
  border-radius: 8px 8px 0 0;
  cursor: pointer;
  font-weight: 600;
  color: #666;
  transition: 0.3s;
}
.tab-item.active {
  background: #fff;
  color: #d32f2f;
  border-top: 3px solid #d32f2f;
}

.tax-portal-container {
  min-height: 100vh;
  background: #f4f7f6;
  font-family: 'Roboto', sans-serif;
}
.tax-header {
  background: #d32f2f;
  color: white;
  padding: 15px 40px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
}
.logo-section {
  display: flex;
  gap: 20px;
  align-items: center;
}
.logo-section h1 {
  margin: 0;
  font-size: 20px;
  letter-spacing: 1px;
}
.logo-section p {
  margin: 0;
  font-size: 12px;
  opacity: 0.9;
}
.tax-content {
  padding: 40px;
  max-width: 1200px;
  margin: 0 auto;
}
.card {
  background: white;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
  overflow: hidden;
}
.card-header {
  padding: 20px;
  border-bottom: 1px solid #eee;
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.card-header h2 {
  margin: 0;
  font-size: 18px;
  color: #333;
}
.tax-table {
  width: 100%;
  border-collapse: collapse;
}
.tax-table th {
  background: #f8f9fa;
  padding: 12px 20px;
  text-align: left;
  font-size: 13px;
  color: #666;
  border-bottom: 2px solid #dee2e6;
}
.tax-table td {
  padding: 15px 20px;
  border-bottom: 1px solid #eee;
  font-size: 14px;
}
.bold {
  font-weight: bold;
}
.text-center {
  text-align: center;
}
.py-20 {
  padding: 40px 0;
}
.ml-8 {
  margin-left: 8px;
}
.btn-refresh {
  background: #fff;
  border: 1px solid #d32f2f;
  color: #d32f2f;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
}
.btn-approve {
  background: #2e7d32;
  color: white;
  border: none;
  padding: 6px 15px;
  border-radius: 4px;
  cursor: pointer;
  font-weight: bold;
}
.btn-approve:hover {
  background: #1b5e20;
}
.btn-reject {
  background: #c62828;
  color: white;
  border: none;
  padding: 6px 15px;
  border-radius: 4px;
  cursor: pointer;
  font-weight: bold;
}
</style>
