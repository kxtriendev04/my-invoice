<script setup>
import { computed, onMounted, onUnmounted, reactive, ref, watch, watchEffect } from 'vue'
import {
  ArrowDown,
  ArrowUp,
  CircleOff,
  Copy,
  Filter,
  Pin,
  PinOff,
  Trash,
  CircleCheck,
  Eye,
} from 'lucide-vue-next'
import MSToolTip from '../ms-tooltip/MSToolTip.vue'
import MSFilterPopup from '../ms-popup/MSFilterPopup.vue'
import { getOperators } from '@/utils/common'
import MSArrowToolTip from '../ms-tooltip/MSArrowToolTip.vue'
import MSCheckbox from '../ms-input/MSCheckbox.vue'

//#region Configuration & Props
const props = defineProps({
  columns: Array,
  tableData: Array,
  activeFilters: Object,
  activeSorts: Array,
  selectedRowIds: Array,
  isLoading: Boolean,
  rowKey: {
    type: String,
    default: 'id',
  },
})

const emit = defineEmits([
  'filter-change',
  'sort-change',
  'select-row',
  'update',
  'duplicate',
  'toggle-status',
  'delete',
  'view',
])
//#endregion

//#region State & Refs
// --- State cho Action Menu
const openActionId = ref(null)
// --- State cho Filter ---
const filterState = reactive({
  column: null,
})
const filterValues = ref({}) // Giá trị lọc nội bộ

// --- State cho Sort ---
const activeSortColumn = ref(null)

// --- State cho Row Selected ---
const activeRow = ref(null)

// --- State Resize Logic ---
const resizingColumn = ref(null) // Cột đang được resize
const startX = ref(0) // Vị trí chuột ban đầu
const startWidth = ref(0) // Độ rộng cột ban đầu

//#endregion

//#region Computed Properties
/**
 * Kiểm tra trạng thái "Chọn tất cả"
 * Trả về true nếu số lượng dòng đã chọn bằng tổng số dòng hiện có
 * createdby: kxtrien - 02.12.2025
 */
const isSelectAll = computed(() => {
  if (!props.tableData || props.tableData.length === 0) return false
  return props.tableData.length > 0 && props.selectedRowIds.length === props.tableData.length
})
//#endregion

//#region Methods

/**
 * Hàm mở/đóng menu của 1 dòng
 */
const toggleActionMenu = (id, event) => {
  event.stopPropagation()

  if (openActionId.value === id) {
    openActionId.value = null
  } else {
    openActionId.value = id
  }
}

/**
 * Xử lý khi click vào 1 item trong menu
 */
const handleMenuAction = (action, item) => {
  openActionId.value = null
  emit(action, item)
}

/**
 * Đóng menu khi click ra ngoài vùng trống bất kỳ
 */
const closeMenuOnClickOutside = () => {
  openActionId.value = null
}

// Lắng nghe sự kiện click toàn màn hình để đóng menu
onMounted(() => {
  window.addEventListener('click', closeMenuOnClickOutside)
})

onUnmounted(() => {
  window.removeEventListener('click', closeMenuOnClickOutside)
})

// Filter Logic (Lọc)
/**
 * Mở Popup lọc hoặc đóng nếu đang mở chính nó
 * @param event
 * @param col - Cấu hình cột
 * createdby: kxtrien - 02.12.2025
 */
const handleOpenFilter = (event, col) => {
  event.stopPropagation()

  // Toggle đóng mở
  if (filterState.column?.prop === col.prop) {
    filterState.column = null
    return
  }

  // Khởi tạo giá trị mặc định nếu chưa có
  if (!filterValues.value[col.prop]) {
    filterValues.value[col.prop] = {
      operator: getOperators(col.type)[0].value,
      value: '',
    }
  }

  filterState.column = col
}

const closeFilter = () => {
  filterState.column = null
}

const applyFilter = () => {
  emit('filter-change', filterValues.value)
  filterState.column = null
}

const clearFilter = (colProp) => {
  if (filterValues.value[colProp]) {
    filterValues.value[colProp].value = ''
    applyFilter()
  }
}

// Sort Logic

/**
 * Kiểm tra xem cột này có đang được sort theo kiểu type không (để active icon)
 * createdby: kxtrien - 02.12.2025
 */
const isSortActive = (col, type) => {
  const sorts = props.activeSorts || []
  const index = sorts.findIndex((s) => s.colProp == col.prop && s.type == type)
  return index != -1
}

/**
 * Xử lý sự kiện sắp xếp
 * @param type - Kiểu sort: 'asc' | 'desc' | null
 * createdby: kxtrien - 02.12.2025
 */
const handleSort = (col, type) => {
  activeSortColumn.value = null
  emit('sort-change', col.prop, type)
}

/**
 * Kiểm tra xem cột

// Selection Logic (Chọn dòng)

/**
 * Xử lý chọn tất cả / bỏ chọn tất cả từ Header
 * createdby: kxtrien - 02.12.2025
 */
const handleSelectAll = (checked) => {
  if (checked) {
    // Lấy tất cả ID của trang hiện tại
    const allIds = (props.tableData || []).map((item) => item[props.rowKey])
    emit('select-row', allIds)
  } else {
    // Gửi mảng rỗng
    emit('select-row', [])
  }
}

/**
 * Xử lý chọn / bỏ chọn từng dòng
 * createdby: kxtrien - 02.12.2025
 */
const handleSelectRow = (rowId, checked) => {
  let newSelectedIds = [...(props.selectedRowIds || [])]

  if (checked) {
    if (!newSelectedIds.includes(rowId)) newSelectedIds.push(rowId)
  } else {
    newSelectedIds = newSelectedIds.filter((id) => id !== rowId)
  }

  emit('select-row', newSelectedIds)
}

/**
 * Xử lý khi click vào 1 dòng (để highlight)
 * createdby: kxtrien - 02.12.2025
 */
const handleClickRow = (id) => {
  activeRow.value = id
}

// Actions (Sửa, Xóa...)
const handleUpdate = (item) => {
  emit('update', item)
}

/**
 * Bắt đầu quá trình resize khi nhấn chuột vào cạnh cột
 * createdby: kxtrien - 08.12.2025
 */
const startResize = (col, event) => {
  // Ngăn chặn sự kiện click lan ra ngoài (để không kích hoạt sort)
  event.stopPropagation()

  resizingColumn.value = col
  startX.value = event.pageX
  startWidth.value = col.width || 100 // Lấy width hiện tại hoặc mặc định

  // Lắng nghe sự kiện di chuyển và nhả chuột trên toàn bộ document
  document.addEventListener('mousemove', handleMouseMove)
  document.addEventListener('mouseup', stopResize)

  // Thêm class để body không bị bôi đen text khi kéo
  document.body.style.cursor = 'col-resize'
  document.body.style.userSelect = 'none'
}

/**
 * Xử lý khi di chuyển chuột để thay đổi độ rộng
 * createdby: kxtrien - 08.12.2025
 */
const handleMouseMove = (event) => {
  if (resizingColumn.value) {
    const diff = event.pageX - startX.value
    // Đặt độ rộng tối thiểu là 50px
    const newWidth = Math.max(10, startWidth.value + diff)
    resizingColumn.value.width = newWidth
  }
}

/**
 * Kết thúc quá trình resize
 * createdby: kxtrien - 08.12.2025
 */
const stopResize = () => {
  resizingColumn.value = null
  document.removeEventListener('mousemove', handleMouseMove)
  document.removeEventListener('mouseup', stopResize)

  // Trả lại style mặc định cho body
  document.body.style.cursor = ''
  document.body.style.userSelect = ''
}
//#endregion

//#region Watchers
/**
 * Đồng bộ Props activeFilters vào State nội bộ filterValues
 * Để khi cha thay đổi (VD: reset filter), con cập nhật theo
 * createdby: kxtrien - 02.12.2025
 */
watch(
  () => props.activeFilters,
  (newVal) => {
    filterValues.value = JSON.parse(JSON.stringify(newVal || {}))
  },
  { deep: true, immediate: true },
)

// Debugger
watchEffect(() => {
  // console.log('filterState', filterState)
})
//#endregion
</script>

<template>
  <div class="table-wrapper">
    <table class="custom-table">
      <thead>
        <tr>
          <th class="sticky-col sticky-left" style="width: 40px; padding: 0">
            <div class="checkbox-center">
              <!-- <input type="checkbox" style="margin: 0" class="input" /> -->
              <MSCheckbox
                style="margin: 0"
                :model-value="isSelectAll"
                @update:model-value="handleSelectAll"
              />
            </div>
          </th>

          <th
            v-for="col in columns"
            :key="col.prop"
            :style="{ width: col.width + 'px', textAlign: col.align }"
            class="th-relative no-select"
          >
            <div class="th-content" :style="{ justifyContent: 'space-between' }">
              <span class="th-text" style="width: 80%" @click="hanleOpenCloseSort(col)"
                >{{ col.label }}
                <div class="">
                  <ArrowUp class="icon" :class="{ active: isSortActive(col, 'asc') }"></ArrowUp>
                  <ArrowDown
                    class="icon"
                    :class="{ active: isSortActive(col, 'desc') }"
                  ></ArrowDown>
                </div>
              </span>

              <!-- Icon filter -->
              <div
                v-if="col.filter"
                class="filter-icon-wrapper"
                :class="{ 'is-active': filterValues[col.prop] && filterValues[col.prop].value }"
                @click="(e) => handleOpenFilter(e, col)"
              >
                <Filter
                  :size="14"
                  style="color: #4b5563"
                  :class="
                    filterValues[col.prop] && filterValues[col.prop].value
                      ? 'icon-filled'
                      : 'icon-stroke'
                  "
                />
              </div>
              <div class="resize-handle" @mousedown="startResize(col, $event)" @click.stop>
                <div class="resize-bar"></div>
              </div>
            </div>

            <!-- Popup filter -->
            <MSFilterPopup
              v-if="
                filterState.column && filterState.column.prop === col.prop && filterValues[col.prop]
              "
              :column="col"
              v-model="filterValues[col.prop]"
              @close="closeFilter"
              @apply="applyFilter"
              @clear="clearFilter(col.prop)"
            />
            <!-- Sort -->
            <div class="sort-pin" v-show="activeSortColumn == col.prop">
              <div class="sort-pin-item" @click="handleSort(col, null)">
                <CircleOff class="icon" />Không sắp xếp
              </div>
              <div
                class="sort-pin-item"
                :class="{ active: isSortActive(col, 'asc') }"
                @click="handleSort(col, 'asc')"
              >
                <ArrowUp class="icon" />Tăng dần
              </div>
              <div
                class="sort-pin-item"
                :class="{ active: isSortActive(col, 'desc') }"
                @click="handleSort(col, 'desc')"
              >
                <ArrowDown class="icon" />Giảm dần
              </div>
              <div class="menu-break"></div>
              <div class="sort-pin-item"><Pin class="icon" />Ghim cột</div>
              <div class="sort-pin-item"><PinOff class="icon" />Bỏ ghim cột</div>
            </div>
          </th>

          <th class="sticky-col sticky-right" style="width: 100px; padding: 0">
            <div class="th-content"></div>
          </th>
        </tr>
      </thead>

      <tbody>
        <!-- Loading Skeleton -->
        <tr v-if="isLoading" v-for="i in 10" :key="i" class="skeleton-row-container">
          <td class="sticky-col sticky-left">
            <div class="td-content justify-center">
              <div class="skeleton-box checkbox"></div>
            </div>
          </td>

          <td v-for="col in columns" :key="col.prop">
            <div class="td-content">
              <div class="skeleton-box text"></div>
            </div>
          </td>

          <td class="sticky-col sticky-right">
            <div class="td-content"></div>
          </td>
        </tr>

        <!-- Dữ liệu -->
        <tr
          v-else
          v-for="item in tableData"
          :key="item[rowKey]"
          @click="handleClickRow(item[rowKey])"
          @dblclick="handleUpdate(item)"
          :class="{
            'is-selected': activeRow === item[rowKey] || selectedRowIds.includes(item[rowKey]),
          }"
        >
          <td class="sticky-col sticky-left">
            <div class="td-content" style="justify-content: center; padding: 0">
              <MSCheckbox
                :model-value="selectedRowIds.includes(item[rowKey])"
                @update:model-value="(val) => handleSelectRow(item[rowKey], val)"
              />
            </div>
          </td>

          <td v-for="col in columns" :key="col.prop">
            <!-- Slot Status, Date -->
            <div class="td-content" v-if="col.custom">
              <slot :name="col.custom" :item="item" :value="item[col.prop]"></slot>
            </div>

            <!-- Slot others -->
            <div
              v-else
              class="td-content"
              :style="{ justifyContent: col.align == 'left' ? 'start' : 'end' }"
            >
              <span>
                <MSToolTip v-if="item[col.prop]" :content="item[col.prop]" position="bottom">
                  {{ item[col.prop] }}
                </MSToolTip>
              </span>
            </div>
          </td>

          <td
            class="sticky-col sticky-right"
            :style="{ zIndex: openActionId === item[rowKey] ? 100 : null, overflow: 'visible' }"
          >
            <div class="td-content" style="cursor: pointer; justify-content: center">
              <div class="icon" @click="handleUpdate(item)">
                <div class="icon-action-pencil"></div>
              </div>
              <div
                class="icon"
                :class="{ 'active-menu': openActionId === item[rowKey] }"
                @click="toggleActionMenu(item[rowKey], $event)"
              >
                <div class="icon-action-more"></div>
              </div>
              <div v-if="openActionId === item[rowKey]" class="row-context-menu" @click.stop>
                <div class="menu-item" @click="handleMenuAction('duplicate', item)">
                  <Copy style="color: #4c5663" :size="14" /> Nhân bản
                </div>

                <div class="menu-item" @click="handleMenuAction('view', item)">
                  <Eye style="color: #4c5663" :size="14" /> Xem
                </div>

                <!-- Logic hiển thị Ngừng sử dụng / Sử dụng -->
                <div class="menu-item" @click="handleMenuAction('toggle-status', item)">
                  <component
                    :is="item.status === 1 ? CircleOff : CircleCheck"
                    :size="14"
                    style="color: #4c5663"
                  />
                  {{ item.status === 1 ? 'Ngừng sử dụng' : 'Sử dụng' }}
                </div>

                <div class="menu-item delete" @click="handleMenuAction('delete', item)">
                  <Trash :size="14" /> Xóa
                </div>
              </div>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
    <div class="grid-no-data" v-if="tableData && tableData.length == 0 && !isLoading">
      <img src="/data-empty.png" alt="" />
      <div class="">Không có dữ liệu</div>
    </div>
  </div>
</template>

<style scoped>
/* --- 1. FILTER ICON STYLES --- */
.filter-icon-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 24px;
  height: 24px;
  border-radius: 4px;
  cursor: pointer;
  color: #9ca3af;
  transition: all 0.2s;
  opacity: 0; /* Ẩn mặc định */
}

/* Hover vào header thì hiện */
.th-content:hover .filter-icon-wrapper {
  opacity: 1;
}

/* Đang lọc thì LUÔN hiện và có màu xanh */
.filter-icon-wrapper.is-active {
  opacity: 1 !important;
  color: #4b5563;
  /* background-color: #e6f6ee; */
}

.icon-filled {
  fill: currentColor;
}
.icon-stroke {
  /* Mặc định lucide dùng stroke */
}

/* --- 2. CẤU HÌNH STICKY & TABLE (Giữ nguyên) --- */
.sticky-col {
  position: sticky !important;
  z-index: 2;
  background-color: #fff;
}
.sticky-left {
  left: 0;
}
.sticky-right {
  right: 0;
}
.td-content {
  gap: 8px;
}
.td-content > .icon {
  width: 24px;
  height: 24px;
  flex-shrink: 0;
  border-radius: 100px;
  background: #ffffff;
  display: flex;
  justify-content: center;
  align-items: center;
}
.td-content > .icon > div {
  color: #4c5663;
}
.td-content > .icon:hover div {
  background-color: var(--color-primary);
}

.custom-table thead th {
  position: sticky;
  top: 0;
  z-index: 10;
  background-color: #f3f4f6;
}
.custom-table thead th.sticky-col {
  z-index: 20 !important;
  background-color: #f3f4f6;
}
.custom-table tbody tr:hover .sticky-col,
.custom-table tbody tr:hover {
  background-color: #f0f6fe;
}

.table-wrapper {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  overflow: auto;
  background: #fff;
}
.custom-table {
  /* width: max-content; */
  /* min-width: 100%; */
  width: 100%; /* Đặt 100% để bảng chiếm hết khung cha */
  min-width: 0;
  table-layout: fixed;
  border-collapse: collapse;
  font-size: 13px;
}
.custom-table tbody tr.is-selected,
.custom-table tbody tr.is-selected .sticky-col {
  background-color: #f0f6fe !important;
}

.custom-table tbody tr.is-selected td,
.custom-table tbody tr.is-selected .sticky-col {
  background-color: #f0f6fe !important;
}
.custom-table tbody tr.is-selected:hover td,
.custom-table tbody tr.is-selected:hover .sticky-col {
  background-color: #f0f6fe !important;
  cursor: pointer;
}

.custom-table th,
.custom-table td {
  padding: 0;
  border-bottom: 1px solid #e5e7eb;
  border-right: none;
  box-sizing: border-box;
  height: 40px;

  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
}
.custom-table thead th {
  line-height: 33px;
  font-weight: 600;
}
.custom-table td {
  height: 36px;
}
.custom-table thead th::after {
  content: '';
  position: absolute;
  top: 50%;
  right: 0;
  transform: translateY(-50%);
  height: 50%;
  width: 0.5px;
  background-color: #d1d5db;
}
.custom-table thead th:has(.resize-handle:hover)::after {
  height: 100%;
}
.checkbox-center {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 !important;
}
input[type='checkbox'] {
  appearance: auto;
  -webkit-appearance: auto;
  width: 16px;
  height: 16px;
  margin: 0;
  padding: 0;
  cursor: pointer;
}

.th-content,
.td-content {
  padding: 0 16px;
  display: flex;
  align-items: center;
  width: 100%;
  height: 100%;
  min-width: 0;

  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
}
.th-content {
  cursor: pointer;
}
.th-text {
  display: flex;
  align-items: center;
  gap: 10px;
}
.th-text > div {
  display: flex;
  align-items: center;
}
.th-text .icon {
  width: 14px;
  color: #4b5563;
  display: none;
}
.th-text .icon.active {
  display: block;
}
.td-content {
  overflow: visible !important;
}
.th-relative {
  position: relative;
  overflow: visible !important;
}
.grid-no-data {
  position: absolute;
  text-align: center;
  font-style: normal;
  color: #111827;
  top: 0;
  left: 50%;
  transform: translate(-50%, 100%);
}
.grid-no-data img {
  overflow: clip;
  width: 180px;
}

.sort-pin {
  position: absolute;
  width: 140px;
  height: 193px;
  padding: 8px 0;
  background-color: #fff;
  box-shadow:
    0 0 8px #0000001a,
    0 8px 16px #0000001a;
  border-radius: 4px;
  animation: slide-down 0.2s ease;
  font-weight: 400;
  line-height: 13px;
}
.sort-pin-item {
  padding: 8px 12px;
  height: 32px;
  display: flex;
  align-items: center;
  color: rgb(17, 24, 39);
  gap: 8px;
}
.sort-pin-item.active {
  color: var(--color-primary) !important;
}
.sort-pin-item .icon {
  color: rgb(75, 85, 99);
  width: 16px;
}
.sort-pin-item.active .icon {
  color: var(--color-primary);
}
.menu-break {
  margin: 8px 12px;
  height: 1px;
  background: #d1d5db;
}
.icon.active-menu {
}

/* Style cho cái Menu Popup */
.row-context-menu {
  position: absolute;
  top: 30px; /* Hiện bên dưới nút 3 chấm */
  right: 16px; /* Căn phải */
  min-width: 108px;
  background: white;
  box-shadow:
    0 0 8px #0000001a,
    0 8px 16px #0000001a;
  border-radius: 4px;
  z-index: 999;
  padding: 8px 0;
  text-align: left;
}

.menu-item {
  height: 32px;
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  cursor: pointer;
}

.menu-item:hover {
  background-color: #f3f4f6;
  color: var(--color-primary);
}

.menu-item.delete {
  color: #ef4444; /* Màu đỏ cho nút xóa */
}

.menu-item.delete:hover {
  background-color: #fee2e2;
}

/* SKELETON CSS */
.skeleton-tr {
  height: 40px;
  border-bottom: 1px solid #e5e7eb;
}
.skeleton-tr .sticky-col {
  background-color: #fff;
  z-index: 1;
}

.skeleton-box {
  background: #e5e7eb;
  border-radius: 4px;
  position: relative;
  overflow: hidden;
}

.skeleton-box::after {
  content: '';
  position: absolute;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  transform: translateX(-100%);
  background-image: linear-gradient(
    90deg,
    rgba(255, 255, 255, 0) 0,
    rgba(255, 255, 255, 0.5) 20%,
    rgba(255, 255, 255, 0.8) 60%,
    rgba(255, 255, 255, 0)
  );
  animation: shimmer 1.5s infinite;
}

@keyframes shimmer {
  100% {
    transform: translateX(100%);
  }
}

.skeleton-box.checkbox {
  width: 16px;
  height: 16px;
  border-radius: 2px;
}

.skeleton-box.text {
  height: 14px;
  width: 80%;
}

.resize-handle {
  position: absolute;
  top: 0;
  right: 0;
  width: 8px;
  height: 100%;
  cursor: col-resize;
  z-index: 30;
  display: flex;
  justify-content: center;
}
/* Xử lý ẩn hiện cột actions */
.custom-table tbody tr td.sticky-right .td-content .icon {
  opacity: 0;
  visibility: hidden;
  transition: all 0.2s ease-in-out;
}

.custom-table tbody tr:hover td.sticky-right .td-content .icon {
  opacity: 1;
  visibility: visible;
}

.custom-table tbody tr.is-selected td.sticky-right .td-content .icon {
  opacity: 1;
  visibility: visible;
}
/* Ẩn hoàn toàn action */
.custom-table tbody tr td.sticky-right .td-content .icon {
  opacity: 0;
  pointer-events: none; /* ❗ quan trọng: không click được khi ẩn */
  transform: translateY(4px); /* optional: hiệu ứng mượt */
  transition: all 0.2s ease;
}

/* Hover vào row thì hiện */
.custom-table tbody tr:hover td.sticky-right .td-content .icon {
  opacity: 1;
  pointer-events: auto;
  transform: translateY(0);
}

/* Nếu row đang selected thì luôn hiện */
.custom-table tbody tr.is-selected td.sticky-right .td-content .icon {
  opacity: 1;
  pointer-events: auto;
  transform: translateY(0);
}
/* Xóa nền trắng mặc định của cột sticky bên phải trong body để không che chữ khi không hover */
.custom-table tbody tr td.sticky-right {
  background-color: transparent !important;
}

/* Đổ lại màu nền khi hover hoặc khi dòng được chọn để làm nền cho các icon action */
.custom-table tbody tr:hover td.sticky-right,
.custom-table tbody tr.is-selected td.sticky-right {
  background-color: #f0f6fe !important;
}
/* Xóa màu nền của cột sticky bên phải ở phần Header */
.custom-table thead th.sticky-right {
  background-color: transparent !important;
}
/* ===== FIX ACTION HOVER ===== */

/* Container làm mốc */
.custom-table tbody tr td.sticky-right .td-content {
  position: relative;
  justify-content: center;
}

/* Group action (bọc 2 icon lại nếu có) */
.custom-table tbody tr td.sticky-right .td-content {
  gap: 0; /* tránh lệch */
}

/* Icon action */
.custom-table tbody tr td.sticky-right .td-content .icon {
  position: absolute;
  top: 50%;
  transform: translateY(-50%) translateX(10px);

  opacity: 0;
  pointer-events: none;

  transition: all 0.2s ease;
}

/* Icon 1 (edit) */
.custom-table tbody tr td.sticky-right .td-content .icon:nth-child(1) {
  right: 36px;
}

/* Icon 2 (more) */
.custom-table tbody tr td.sticky-right .td-content .icon:nth-child(2) {
  right: 8px;
}

/* Hover row → hiện */
.custom-table tbody tr:hover td.sticky-right .td-content .icon {
  opacity: 1;
  pointer-events: auto;
  transform: translateY(-50%) translateX(0);
}

/* Selected → luôn hiện */
.custom-table tbody tr.is-selected td.sticky-right .td-content .icon {
  opacity: 1;
  pointer-events: auto;
  transform: translateY(-50%) translateX(0);
}
</style>
