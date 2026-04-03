<template>
  <Transition name="slide-down">
    <div v-if="toast.isShow" class="ms-toast" :class="toast.type">
      <div class="ms-toast-left-line"></div>
      <div class="container">
        <div class="main">
          <CircleCheck v-if="toast.type === 'success'" class="check" :size="18" />
          <AlertCircle v-else-if="toast.type === 'error'" class="check" :size="18" />
          <AlertTriangle v-else-if="toast.type === 'warning'" class="check" :size="18" />
          {{ toast.message }}
        </div>
        <X @click="toast.closeToast" :size="20" style="color: #4d5764; cursor: pointer" />
      </div>
    </div>
  </Transition>
</template>

<script setup>
import { CircleCheck, X, AlertCircle, AlertTriangle } from 'lucide-vue-next'
import { useToastStore } from '@/stores/useToastStore' // Đường dẫn tới store vừa tạo

const toast = useToastStore()
</script>

<style scoped>
.ms-toast {
  position: fixed;
  top: 20px;
  right: 20px;
  min-width: 300px;
  height: 48px;
  background: #fff;
  border-radius: 4px;
  box-shadow: 0 3px 6px rgba(0, 0, 0, 0.16);
  display: flex;
  align-items: center;
  z-index: 9999;
  overflow: hidden;
}

.ms-toast-left-line {
  width: 6px;
  height: 100%;
  flex-shrink: 0;
}

/* Màu sắc theo Type */
.ms-toast.success .ms-toast-left-line {
  background-color: #28a745;
}
.ms-toast.error .ms-toast-left-line {
  background-color: #dc3545;
}
.ms-toast.warning .ms-toast-left-line {
  background-color: #ffc107;
}

.ms-toast.success .check {
  color: #28a745;
}
.ms-toast.error .check {
  color: #dc3545;
}
.ms-toast.warning .check {
  color: #ffc107;
}

.container {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  padding: 0 16px;
}

.main {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  color: #111;
}

/* Animation */
.slide-down-enter-active,
.slide-down-leave-active {
  transition: all 0.4s ease;
}

.slide-down-enter-from,
.slide-down-leave-to {
  transform: translateY(-100%);
  opacity: 0;
}
</style>
