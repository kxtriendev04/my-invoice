<script setup>
import { computed } from 'vue'
import { ChevronFirst, ChevronLast, ChevronLeft, ChevronRight } from 'lucide-vue-next'
import MSDropDown from '../ms-dropdown/MSDropDown.vue'

const props = defineProps({
  totalRecords: { type: Number, default: 0 },
  pageSize: { type: Number, default: 20 },
  pageIndex: { type: Number, default: 1 },
  pageSizeOptions: { type: Array, default: () => [10, 20, 30, 50, 100] },
})

const emit = defineEmits(['update:pageSize', 'update:pageIndex'])

/**
 * Tính tổng số trang
 */
const totalPages = computed(() => {
  if (props.pageSize === 0) return 0
  return Math.ceil(props.totalRecords / props.pageSize)
})

/**
 * Tính toán hiển thị khoảng bản ghi (VD: 1 - 20)
 */
const recordRange = computed(() => {
  if (props.totalRecords === 0) return '0 - 0'
  const from = (props.pageIndex - 1) * props.pageSize + 1
  const to = Math.min(props.pageIndex * props.pageSize, props.totalRecords)
  return `${from} - ${to}`
})

/**
 * Xử lý khi thay đổi số dòng trên trang
 */
const handlePageSizeChange = (val) => {
  emit('update:pageSize', val)
  emit('update:pageIndex', 1) // Reset về trang 1 khi đổi số dòng
}

/**
 * Chuyển trang
 */
const setPage = (page) => {
  if (page >= 1 && page <= totalPages.value) {
    emit('update:pageIndex', page)
  }
}
</script>

<template>
  <div class="ms-pagination">
    <div class="pagination-count">
      Tổng số:
      <h6>{{ totalRecords }}</h6>
    </div>
    <div class="main-pagination">
      <span>Số dòng/trang</span>
      <MSDropDown
        :model-value="pageSize"
        @update:model-value="handlePageSizeChange"
        :options="pageSizeOptions"
      />
      <p>{{ recordRange }}</p>
      <div class="btn-page">
        <button @click="setPage(1)" :disabled="pageIndex === 1">
          <ChevronFirst class="icon" />
        </button>
        <button @click="setPage(pageIndex - 1)" :disabled="pageIndex === 1">
          <ChevronLeft class="icon" />
        </button>
        <button @click="setPage(pageIndex + 1)" :disabled="pageIndex === totalPages">
          <ChevronRight class="icon" />
        </button>
        <button @click="setPage(totalPages)" :disabled="pageIndex === totalPages">
          <ChevronLast class="icon" />
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.ms-pagination {
  height: 44px;
  padding: 0 16px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-top: 1px solid #e5e7eb;
  background: #fff;
  font-size: 13px;
}
.pagination-count {
  display: flex;
  gap: 4px;
  align-items: center;
}
.pagination-count h6 {
  font-weight: 700;
  margin: 0;
}
.main-pagination {
  display: flex;
  align-items: center;
  gap: 16px;
}
.main-pagination p {
  font-weight: 700;
  margin: 0;
}
.btn-page {
  display: flex;
}
.btn-page button {
  background: transparent;
  border: none;
  cursor: pointer;
  padding: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
}
.btn-page button:disabled {
  opacity: 0.3;
  cursor: not-allowed;
}
.btn-page button .icon {
  width: 16px;
}
</style>
