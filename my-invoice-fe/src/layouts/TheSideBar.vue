<script setup>
import { ref, watch, markRaw } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import {
  ChevronDown,
  ChevronRight,
  Pencil,
  Monitor,
  BadgeCheck,
  FileText,
  FileWarning,
  ArrowRightLeft,
  PieChart,
  Box,
  Settings,
  Landmark,
  Shield,
  Building2,
  Users,
  UserPlus,
} from 'lucide-vue-next'

//#region const
const sidebarList = [
  // 1. Link thường
  {
    id: 'dashboard',
    label: 'Bàn làm việc',
    icon: markRaw(Monitor),
    type: 'link',
    to: '/',
  },
  {
    id: 'register',
    label: 'Đăng ký phát hành',
    icon: markRaw(BadgeCheck),
    type: 'accordion',
    children: [
      { label: 'Lập Tờ khai', to: '/invoice/declaration/list', type: 'link' },
      { label: 'Mẫu hoá đơn', to: '/invoice/template/list', type: 'link' },
    ],
  },
  {
    id: 'invoice',
    label: 'Hóa đơn',
    icon: markRaw(FileText),
    type: 'accordion',
    children: [
      { label: 'Hoá đơn', to: '/invoice/list', type: 'link' },
      { label: 'HĐ từ máy tính tiền', to: '/invoice/pos', type: 'link' },
      { label: 'Phiếu xuất kho', to: '/invoice/export', type: 'link' },
    ],
  },
  {
    id: 'wrong-invoice',
    label: 'Xử lý HĐ lập sai',
    icon: markRaw(FileWarning),
    type: 'link',
    to: '/wrong-invoice',
    badge: { text: '12', type: 'danger' }, // Badge màu đỏ
  },
  {
    id: 'history',
    label: 'Lịch sử truyền nhận',
    icon: markRaw(ArrowRightLeft),
    type: 'link',
    to: '/history',
  },
  {
    id: 'report',
    label: 'Báo cáo',
    icon: markRaw(PieChart),
    type: 'link',
    to: '/report',
  },
  {
    id: 'category',
    label: 'Danh mục',
    icon: markRaw(Box),
    type: 'link',
    to: '/category',
  },
  {
    id: 'break-1',
    type: 'break',
  },
  {
    id: 'admin', // ID này phải trùng với logic nhận diện
    label: 'Quản trị hệ thống',
    icon: markRaw(Settings),
    type: 'accordion',
    children: [
      { label: 'Quản lý công ty', to: '/admin/companies', type: 'link' },
      { label: 'Quản lý tài khoản', to: '/admin/users', type: 'link' },
      { label: 'Gán tài khoản vào công ty', to: '/admin/assign-user', type: 'link' },
    ],
  },
  {
    id: 'system',
    label: 'Cấu hình',
    icon: markRaw(Settings),
    type: 'link',
    to: '/system',
  },
  {
    id: 'loan',
    label: 'Kết nối vay vốn',
    icon: markRaw(Landmark),
    type: 'link',
    to: '/loan',
  },
  {
    id: 'insurance',
    label: 'Kết nối bảo hiểm',
    icon: markRaw(Shield),
    type: 'link',
    to: '/insurance',
    badge: { text: 'Mới', type: 'warning' }, // Badge màu cam
  },
]
//#endregion

//#region Hooks
const router = useRouter()
const route = useRoute()
//#endregion

//#region State
const expandedId = ref('invoice')
const activeMainId = ref('invoice')
const isCollapsed = ref(false)
//#endregion

/**
 * Hàm kiểm tra active (So khớp đường dẫn)
 */
const checkIsActive = (path) => {
  if (!path) return false
  if (path === '/') return route.path === '/'
  return route.path.startsWith(path)
}

/**
 * Hàm kiểm tra cha active
 */
const checkParentActive = (item) => {
  if (item.children) {
    return item.children.some((child) => {
      // Kiểm tra xem URL hiện tại có bắt đầu bằng đường dẫn của con không
      return route.path.startsWith(child.to)
    })
  }
  return false
}

/**
 * Hàm kiểm chuyển hướng tới trang yêu cầu
 */
const navigateTo = (path) => path && router.push(path)

/**
 * Hàm xử lý click vào item trong sidebar
 */
const handleItemClick = (item) => {
  if (item.type === 'flyout') return // Chặn click nếu là FLyout

  if (item.type === 'accordion') {
    if (isCollapsed.value) return

    if (expandedId.value === item.id) {
      expandedId.value = null
    } else {
      expandedId.value = item.id
      activeMainId.value = item.id
    }
  } else {
    activeMainId.value = item.id
    navigateTo(item.to)
  }
}

watch(
  [() => route.path, isCollapsed],
  () => {
    sidebarList.forEach((item) => {
      const isSelfActive = checkIsActive(item.to)
      const isParentActive =
        (item.type === 'accordion' || item.type === 'flyout') && checkParentActive(item)

      if (isSelfActive || isParentActive) {
        activeMainId.value = item.id
        if (item.type === 'accordion' && !isCollapsed.value) {
          expandedId.value = item.id
        }
      }
    })
  },
  { immediate: true },
)

const handleClickCollapseButton = () => {
  isCollapsed.value = !isCollapsed.value
}
</script>

<template>
  <div class="sidebar no-select" :class="{ collapsed: isCollapsed }">
    <div class="sidebar-list">
      <div class="sidebar-item" v-for="sidebar in sidebarList" :key="sidebar.id">
        <div v-if="sidebar.type == 'break'" class="sidebar-break"></div>
        <div
          v-else
          class="sidebar-row"
          :class="{
            expanded: expandedId === sidebar.id,
            active: activeMainId === sidebar.id && sidebar.type !== 'accordion',
            'accordion-active': activeMainId === sidebar.id && sidebar.type === 'accordion',
          }"
          @click="handleItemClick(sidebar)"
        >
          <div class="sidebar-row-left">
            <component :is="sidebar.icon" :size="20" class="sidebar-icon" />
            <span v-if="!isCollapsed">{{ sidebar.label }}</span>
          </div>

          <div class="sidebar-row-right" v-show="!isCollapsed">
            <span v-if="sidebar.badge" class="sidebar-badge" :class="`badge-${sidebar.badge.type}`">
              {{ sidebar.badge.text }}
            </span>

            <ChevronDown
              v-if="sidebar.type === 'accordion'"
              class="arrow-icon"
              :class="{ rotated: expandedId === sidebar.id }"
              :size="16"
            />

            <ChevronRight v-if="sidebar.type === 'flyout'" class="arrow-icon" :size="16" />
          </div>
        </div>

        <div
          v-if="sidebar.type === 'accordion'"
          v-show="expandedId === sidebar.id && !isCollapsed"
          class="accordion-wrapper"
        >
          <div class="sidebar-sub-item" v-for="subItem in sidebar.children" :key="subItem.label">
            <div
              class="sub-item-row"
              :class="{ active: checkIsActive(subItem.to) }"
              @click.stop="navigateTo(subItem.to)"
            >
              <span>{{ subItem.label }}</span>

              <ChevronRight
                v-if="subItem.type === 'flyout'"
                :size="16"
                class="sub-item-icon-right arrow-icon"
              />
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="scale-container">
      <div class="footer-btn" v-show="!isCollapsed">
        <Pencil :size="16" class="footer-icon" />
      </div>
      <div class="footer-btn" @click="handleClickCollapseButton" style="margin-left: auto">
        <div class="icon-sidebar-scale footer-icon" :class="{ rotated: isCollapsed }"></div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.sidebar {
  width: 230px;
  height: 100vh;
  background: #ffffff;
  display: flex;
  flex-direction: column;
  color: #374151;
  font-size: 13px;
  font-family: 'Inter', sans-serif;
  border-right: 1px solid #e5e7eb;
  transition: width 0.3s ease;
  padding: 16px;
}
.sidebar.collapsed {
  width: 60px !important;
}

.sidebar-list {
  flex: 1;
  overflow-y: auto;
  overflow-x: hidden;
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.sidebar-list::-webkit-scrollbar {
  width: 4px;
}
.sidebar-list::-webkit-scrollbar-thumb {
  background-color: #d1d5db;
  border-radius: 4px;
}

.sidebar-item {
  position: relative;
}

.sidebar-break {
  margin: 8px auto;
  width: 100%;
  border-bottom: 1px solid #f3f4f6;
}

/* --- DÒNG CHA --- */
.sidebar-row {
  border-radius: 6px;
  padding: 8px 12px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  cursor: pointer;
  height: 40px; /* Nhỉnh lên chút cho icon đẹp hơn */
  transition: all 0.2s ease;
  white-space: nowrap;
  color: #4b5563; /* Xám sẫm mượt mà hơn */
}

.sidebar-row-left {
  display: flex;
  align-items: center;
  gap: 12px; /* Khoảng cách giữa icon và chữ */
}
.sidebar-row-left span {
  font-weight: 500;
}

/* CSS cho component Icon SVG */
.sidebar-icon {
  color: #6b7280; /* Màu mặc định của icon */
  transition: color 0.2s ease;
  min-width: 20px;
}

.sidebar-row-right {
  display: flex;
  align-items: center;
  gap: 8px;
}

/* Các trạng thái Hover / Active */
.sidebar-row:hover {
  background-color: #f3f4f6;
}

/* Link Active hoặc Accordion đang mở */
.sidebar-row.active,
.sidebar-row.expanded {
  background-color: #eff6ff !important;
  color: #2563eb !important;
}

/* Đổi màu Icon sang Xanh MISA khi Hover/Active */
.sidebar-row.active .sidebar-icon,
.sidebar-row.expanded .sidebar-icon {
  color: #2563eb !important;
}

/* Mũi tên Dropdown */
.arrow-icon {
  color: #9ca3af;
  transition:
    transform 0.3s ease,
    color 0.2s ease;
}
.sidebar-row:hover .arrow-icon {
  color: #6b7280;
}
.sidebar-row.active .arrow-icon,
.sidebar-row.expanded .arrow-icon {
  color: #2563eb !important;
}
.arrow-icon.rotated {
  transform: rotate(180deg);
}

/* --- BADGE (NHÃN SỐ LƯỢNG) --- */
.sidebar-badge {
  padding: 2px 6px;
  border-radius: 4px;
  font-size: 11px;
  font-weight: 600;
  color: #ffffff;
  line-height: 1;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}
.badge-danger {
  background-color: #ef4444;
}
.badge-warning {
  background-color: #f97316;
}

/* --- MENU CON (ACCORDION) --- */
.accordion-wrapper {
  margin-top: 2px;
}

.sub-item-row {
  border-radius: 6px;
  display: flex;
  align-items: center;
  padding: 8px 12px;
  cursor: pointer;
  color: #6b7280;
  min-height: 36px;
  position: relative;
  padding-left: 44px; /* Thụt vào thẳng hàng với chữ của menu cha */
  transition: background-color 0.2s;
}

.sub-item-row span {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  flex: 1;
}

.sub-item-row:hover {
  background-color: #f3f4f6;
  color: #111827;
}

.sub-item-row.active {
  color: #2563eb;
  font-weight: 500;
}

/* --- FOOTER --- */
.scale-container {
  box-sizing: border-box;
  width: 100%;
  height: 52px;
  padding: 0 0;
  display: flex;
  align-items: center;
  justify-content: space-between;
  background: #ffffff;
  border-top: 1px solid #e5e7eb;
  margin-top: auto;
}

.footer-btn {
  width: 32px;
  height: 32px;
  border-radius: 6px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 1px solid transparent;
  transition: background-color 0.2s;
}
.footer-btn:hover {
  background-color: #f3f4f6;
  border-color: #e5e7eb;
}

.footer-icon {
  color: #6b7280;
}

.icon-sidebar-scale {
  /* Nếu bạn thay thế nút này bằng icon Lucide thì đổi luôn, ở đây mình giữ nguyên icon cũ của bạn */
  background-color: #6b7280;
  width: 16px;
  height: 16px;
  transition: transform 0.3s ease;
  mask-image: url('data:image/svg+xml;utf8,<svg ... />'); /* Thay lại src của bạn */
}
.icon-sidebar-scale.rotated {
  transform: rotate(180deg);
}
</style>
