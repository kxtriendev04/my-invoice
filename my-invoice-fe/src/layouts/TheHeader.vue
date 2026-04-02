<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import {
  Grip,
  ChevronDown,
  Settings,
  Bot,
  MessageCircleMore,
  Bell,
  CircleHelp,
  CircleEllipsis,
  LayoutGrid,
  LogOut,
  User,
} from 'lucide-vue-next'
import defaultAvatar from '/defaultAvatar.jpg'
import MSArrowToolTip from '@/components/ms-tooltip/MSArrowToolTip.vue'
import { useRouter } from 'vue-router'

const router = useRouter()

// 1. Lấy thông tin từ localStorage
const user = ref(JSON.parse(localStorage.getItem('user') || '{}'))
const company = ref(JSON.parse(localStorage.getItem('company') || '{}'))
// 2. Trạng thái đóng/mở menu
const isShowMenu = ref(false)

const toggleMenu = (event) => {
  event.stopPropagation()
  isShowMenu.value = !isShowMenu.value
}

const closeMenu = () => {
  isShowMenu.value = false
}

const handleLogout = () => {
  localStorage.clear()
  router.push('/login')
}

onMounted(() => {
  window.addEventListener('click', closeMenu)
})
onUnmounted(() => {
  window.removeEventListener('click', closeMenu)
})
</script>

<template>
  <header>
    <div class="header-left">
      <div class="icon-btn">
        <Grip :size="24" />
      </div>

      <div class="app-logo">
        <img src="/app-logo.png" alt="" style="width: 14px" />
        <!-- <div class="misa-icon-placeholder"></div> -->
      </div>

      <h3 class="logo-title">MyInvoice</h3>

      <div class="company cursor-pointer">
        <span class="company-name">{{ company?.companyName || 'Đơn vị chưa xác định' }}</span>
      </div>

      <div class="db-selector cursor-pointer">
        <span class="status-dot"></span>
        <span class="db-year">Dữ liệu năm 2026</span>
        <ChevronDown :size="16" />
      </div>
    </div>

    <div class="header-right">
      <div class="xlhd-badge cursor-pointer">
        <div class="xlhd-icon"></div>
        <div class="xlhd-text">
          <div class="xlhd-title">MyInvoice</div>
          <div class="xlhd-sub">XỬ LÝ HÓA ĐƠN</div>
        </div>
      </div>

      <div class="header-actions">
        <MSArrowToolTip position="bottom" content="Thiết lập">
          <div class="icon-btn"><Settings :size="20" /></div>
        </MSArrowToolTip>

        <MSArrowToolTip position="bottom" content="MISA AVA">
          <div class="icon-btn"><Bot :size="20" /></div>
        </MSArrowToolTip>

        <MSArrowToolTip position="bottom" content="Tin nhắn">
          <div class="icon-btn relative">
            <MessageCircleMore :size="20" />
            <span class="notification-badge">20</span>
          </div>
        </MSArrowToolTip>

        <MSArrowToolTip position="bottom" content="Thông báo">
          <div class="icon-btn"><Bell :size="20" /></div>
        </MSArrowToolTip>

        <MSArrowToolTip position="bottom" content="Trợ giúp">
          <div class="icon-btn"><CircleHelp :size="20" /></div>
        </MSArrowToolTip>

        <MSArrowToolTip position="bottom" content="Tính năng khác">
          <div class="icon-btn"><CircleEllipsis :size="20" /></div>
        </MSArrowToolTip>

        <MSArrowToolTip position="bottom" content="Ứng dụng khác">
          <div class="icon-btn"><LayoutGrid :size="20" /></div>
        </MSArrowToolTip>
      </div>

      <!-- Avatar và Dropdown Logic -->
      <div class="user-menu-wrapper">
        <img :src="defaultAvatar" alt="avt" class="avt" @click="toggleMenu" />

        <div v-if="isShowMenu" class="account-dropdown" @click.stop>
          <div class="dropdown-info">
            <div class="info-name">{{ user.fullName }}</div>
            <div class="info-tax">MST: {{ company.companyTaxCode }}</div>
          </div>
          <div class="dropdown-item" @click="router.push('/admin/user-profile')">
            <User :size="16" /> Thiết lập tài khoản
          </div>
          <div class="dropdown-item logout" @click="handleLogout">
            <LogOut :size="16" /> Đăng xuất
          </div>
        </div>
      </div>
    </div>
  </header>
</template>

<style scoped>
header {
  height: 48px;
  width: 100%;
  background-color: #2563eb; /* Màu xanh MISA, bạn có thể chỉnh mã HEX #1961D2 nếu cần chuẩn xác hơn */
  color: #ffffff;
  padding: 0 16px;
  display: flex;
  flex-shrink: 0;
  align-items: center;
  justify-content: space-between;
  font-family: 'Inter', sans-serif;
}

/* --- TRÁI --- */
.header-left {
  display: flex;
  align-items: center;
}

.icon-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  cursor: pointer;
  transition: background-color 0.2s;
  color: #ffffff;
}
.icon-btn:hover {
  background-color: rgba(255, 255, 255, 0.15);
}

.app-logo {
  width: 32px;
  height: 32px;
  background-color: #5884e4;
  border-radius: 11px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-left: 8px;
  overflow: hidden;
}
.misa-icon-placeholder {
  /* Bạn thêm background-image logo MISA trắng vào đây */
  width: 18px;
  height: 18px;
  background-color: #fff;
  mask-image: url('path-to-your-misa-logo.svg'); /* Tùy chọn */
  mask-size: contain;
}

.logo-title {
  margin-left: 12px;
  font-weight: 600;
  font-size: 16px;
  cursor: pointer;
  margin-right: 24px;
}

.company {
  display: flex;
  align-items: center;
  gap: 4px;
  margin-right: 16px;
}
.company-name {
  font-size: 14px;
  max-width: 250px;
  text-overflow: ellipsis;
  overflow: hidden;
  white-space: nowrap;
}

.db-selector {
  display: flex;
  align-items: center;
  background-color: #4b83f2;
  border-radius: 6px;
  padding: 4px 10px;
  gap: 6px;
  font-size: 13px;
  transition: background-color 0.2s;
}
.db-selector:hover {
  background-color: #5c90f5;
}
.status-dot {
  width: 8px;
  height: 8px;
  background-color: #10b981; /* Chấm xanh lá */
  border-radius: 50%;
}
.db-year {
  margin-right: 4px;
}

/* --- PHẢI --- */
.header-right {
  display: flex;
  align-items: center;
}

.xlhd-badge {
  display: flex;
  align-items: center;
  background-color: rgba(255, 255, 255, 0.15);
  border-radius: 6px;
  padding: 4px 12px;
  margin-right: 16px;
  transition: background-color 0.2s;
}
.xlhd-badge:hover {
  background-color: rgba(255, 255, 255, 0.25);
}
.xlhd-icon {
  width: 24px;
  height: 24px;
  background-color: #10b981;
  border-radius: 50%;
  margin-right: 8px;
  /* Thêm icon MISA xanh lá vào đây */
}
.xlhd-text {
  display: flex;
  flex-direction: column;
}
.xlhd-title {
  font-size: 12px;
  font-weight: 600;
  line-height: 1.2;
}
.xlhd-sub {
  font-size: 10px;
  opacity: 0.9;
  line-height: 1.2;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 4px;
}

.relative {
  position: relative;
}
.notification-badge {
  position: absolute;
  top: 0px;
  right: -2px;
  background-color: #ef4444; /* Đỏ thông báo */
  color: white;
  font-size: 10px;
  font-weight: bold;
  height: 16px;
  min-width: 16px;
  padding: 0 4px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 1px solid #2563eb; /* Viền tiệp màu nền để nổi bật */
}

.avt {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  margin-left: 12px;
  object-fit: cover;
  cursor: pointer;
}

.cursor-pointer {
  cursor: pointer;
}

/* --- LOGIC MENU BỔ SUNG (KHÔNG SỬA STYLE CŨ) --- */
.user-menu-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.account-dropdown {
  position: absolute;
  top: calc(100% + 10px);
  right: 0;
  background: white;
  border-radius: 4px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  width: 220px;
  z-index: 9999;
  color: #333;
  padding: 8px 0;
}

.dropdown-info {
  padding: 8px 16px;
}
.info-name {
  font-weight: 700;
  font-size: 14px;
}
.info-tax {
  font-size: 12px;
  color: #666;
}
.dropdown-divider {
  height: 1px;
  background: #eee;
  margin: 4px 0;
}
.dropdown-item {
  padding: 10px 16px;
  display: flex;
  align-items: center;
  gap: 10px;
  cursor: pointer;
  font-size: 13px;
}
.dropdown-item:hover {
  background-color: #f3f4f6;
}
.dropdown-item.logout {
  color: #ef4444;
}
</style>
