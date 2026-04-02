<script setup>
import { getOperators } from '@/utils/common'
import { computed } from 'vue'
import MSDatePicker from '../ms-input/MSDatePicker.vue'

//#region Props & Emit
const props = defineProps({
  column: {
    type: Object,
    required: true,
  },
  modelValue: {
    type: Object,
    default: () => ({ operator: 'contains', value: '' }),
  },
})
const emit = defineEmits(['update:modelValue', 'close', 'apply', 'clear'])
//#end region

//#region Computed
/**
 * Computed property xử lý v-model 2 chiều cho object filter
 * Giúp tránh mutate trực tiếp props (Anti-pattern trong Vue)
 * createdby: kxtrien - 02.12.2025
 */
const filterData = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})
//#endregion

//#region Methods
/**
 * Xử lý sự kiện đóng Popup
 * createdby: kxtrien - 02.12.2025
 */
const onClose = () => emit('close')

/**
 * Xử lý sự kiện áp dụng bộ lọc (Apply)
 * createdby: kxtrien - 02.12.2025
 */
const onApply = () => emit('apply')

/**
 * Xử lý sự kiện bỏ lọc (Clear)
 * createdby: kxtrien - 02.12.2025
 */
const onClear = () => emit('clear')
//#endregion
</script>

<template>
  <div class="filter-popup" @click.stop>
    <div class="filter-header">
      <span>Lọc {{ column.label }}</span>
      <span class="close-btn" @click="onClose">✕</span>
    </div>

    <div class="filter-body">
      <select v-model="filterData.operator" class="custom-input mb-2">
        <option v-for="op in getOperators(column.type)" :key="op.value" :value="op.value">
          {{ op.label }}
        </option>
      </select>

      <MSDatePicker
        v-if="column.type === 'date' && !['empty', 'not_empty'].includes(filterData.operator)"
        v-model="filterData.value"
        @keydown.enter="onApply"
      />

      <input
        v-else-if="!['empty', 'not_empty'].includes(filterData.operator)"
        type="text"
        class="custom-input"
        placeholder="Nhập giá trị..."
        v-model="filterData.value"
        @keydown.enter="onApply"
      />
    </div>

    <div class="filter-footer">
      <button class="btn-text" @click="onClear">Bỏ lọc</button>
      <div>
        <button class="btn-outline" @click="onClose">Hủy</button>
        <button class="btn-primary" @click="onApply">Áp dụng</button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.filter-popup {
  position: absolute;
  top: 100%;
  left: 0;
  z-index: 1000;
  background: white;
  width: 300px;
  border-radius: 4px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
  border: 1px solid #ebeef5;
  font-weight: normal;
  text-align: left;
  color: #333;
  cursor: default;
  padding: 16px;
}

.filter-header {
  display: flex;
  justify-content: space-between;
  font-weight: 600;
  margin-bottom: 12px;
}

.filter-body {
  margin-bottom: 16px;
  display: flex;
  flex-direction: column;
}

.filter-footer {
  display: flex;
  justify-content: space-between;
}

.custom-input {
  width: 100%;
  height: 32px;
  padding: 0 8px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  outline: none;
  box-sizing: border-box;
}
.custom-input:focus {
  border-color: #00a65a;
}
.mb-2 {
  margin-bottom: 8px;
}

.btn-text {
  background: none;
  border: none;
  color: #666;
  cursor: pointer;
}
.btn-text:hover {
  color: #00a65a;
}

.btn-outline {
  background: white;
  border: 1px solid #ddd;
  padding: 6px 12px;
  border-radius: 4px;
  cursor: pointer;
  margin-right: 8px;
}
.btn-primary {
  background: #00a65a;
  color: white;
  border: none;
  padding: 6px 12px;
  border-radius: 4px;
  cursor: pointer;
}
.close-btn {
  cursor: pointer;
  color: #999;
}
</style>
