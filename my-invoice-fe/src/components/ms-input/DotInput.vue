<script setup>
import { useField } from 'vee-validate'

const props = defineProps({
  name: { type: String, required: true },
  placeholder: { type: String, default: '' },
  type: { type: String, default: 'text' },
})

const { value, errorMessage } = useField(() => props.name)
</script>

<template>
  <div class="misa-input-wrapper">
    <input
      v-model="value"
      :type="type"
      :placeholder="placeholder"
      class="misa-input"
      :class="{ 'is-invalid': errorMessage }"
    />
    <span v-if="errorMessage" class="error-text">{{ errorMessage }}</span>
  </div>
</template>

<style scoped>
.misa-input-wrapper {
  position: relative;
  width: 100%;
  display: flex;
  flex-direction: column;
}
.misa-input {
  width: 100%;
  height: 26px;
  font-size: 13px;
  color: #111827;
  background: transparent;
  border: none;
  border-bottom: 1px dotted #9ca3af; /* Viền nét đứt đặc trưng MISA */
  outline: none;
  padding: 0 4px;
  transition: border-color 0.2s;
}
.misa-input:focus {
  border-bottom: 1px solid #2563eb; /* Khi focus thì nét liền màu xanh */
}
.misa-input.is-invalid {
  border-bottom: 1px solid #ef4444; /* Viền đỏ khi lỗi */
}
.error-text {
  color: #ef4444;
  font-size: 11px;
  position: absolute;
  top: 100%;
  left: 0;
  margin-top: 2px;
}
</style>
