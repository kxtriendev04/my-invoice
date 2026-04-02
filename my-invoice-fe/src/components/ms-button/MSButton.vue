<template>
  <button :class="type"><slot></slot></button>
</template>
<script setup>
//#region Props
const props = defineProps({
  type: {
    type: String,
    default: 'primary',
  },
})
//#endregion
</script>
<style scoped>
button {
  padding: 6px 12px;
  border-radius: 4px;
  border: none;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 4px;
  font-weight: 500;
  transition: all 0.2s ease;
  cursor: pointer;
}
.primary {
  background-color: var(--color-primary);
  color: #ffffff;
}
.primary:hover {
  background-color: #007b5d;
}
.secondary {
  background-color: #fff;
  border: 1px solid #d1d5db;
  color: #111827;
}
.secondary:hover {
  background-color: #f3f4f6;
}
</style>
