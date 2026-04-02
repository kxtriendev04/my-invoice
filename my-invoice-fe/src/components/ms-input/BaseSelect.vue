<script setup>
import { useField } from 'vee-validate'
import { ChevronDown } from 'lucide-vue-next'

const props = defineProps({
  name: { type: String, required: true },
  options: { type: Array, default: () => [] },
})

const { value, errorMessage } = useField(() => props.name)
</script>

<template>
  <div class="misa-select-wrapper">
    <select v-model="value" class="misa-select" :class="{ 'is-invalid': errorMessage }">
      <option value="" disabled hidden></option>
      <option v-for="opt in options" :key="opt.value" :value="opt.value">
        {{ opt.label }}
      </option>
    </select>
    <ChevronDown class="select-icon" :size="14" />
    <span v-if="errorMessage" class="error-text">{{ errorMessage }}</span>
  </div>
</template>

<style scoped>
.misa-select-wrapper {
  position: relative;
  width: 100%;
}
.misa-select {
  width: 100%;
  height: 26px;
  font-size: 13px;
  color: #111827;
  background: transparent;
  border: none;
  border-bottom: 1px dotted #9ca3af;
  outline: none;
  padding: 0 20px 0 4px;
  appearance: none; /* Ẩn mũi tên mặc định */
  cursor: pointer;
}
.misa-select:focus {
  border-bottom: 1px solid #2563eb;
}
.misa-select.is-invalid {
  border-bottom: 1px solid #ef4444;
}
.select-icon {
  position: absolute;
  right: 4px;
  top: 50%;
  transform: translateY(-50%);
  color: #6b7280;
  pointer-events: none;
}
.error-text {
  color: #ef4444;
  font-size: 11px;
  position: absolute;
  top: 100%;
  left: 0;
}
</style>
