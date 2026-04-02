<template>
  <Transition name="slide-down">
    <div v-if="isShow" class="ms-toast" @mouseenter="pauseTimer" @mouseleave="resumeTimer">
      <div class="ms-toast-left-line"></div>

      <div class="container">
        <div class="main"><CircleCheck class="check" :size="18" />{{ toastMessage }}</div>
        <X @click="closeToast" :size="20" style="color: #4d5764; cursor: pointer" />
      </div></div
  ></Transition>
</template>

<script setup>
import { CircleCheck, X } from 'lucide-vue-next'
import { ref, Transition } from 'vue'

//#region State
const isShow = ref(false)
const toastMessage = ref('')
let timeout = null
//#end region

/**
 * Hiển thị Toast thông báo thành công.
 * Hàm này được expose ra ngoài cho component cha sử dụng.
 * @param message - Nội dung thông báo (mặc định: 'Thành công')
 * createdby: kxtrien - 02.12.2025
 */
const showSuccessToast = (message = 'Thành công') => {
  toastMessage.value = message
  isShow.value = true

  startTimer()
}
/**
 * Bắt đầu bộ đếm ngược để ẩn Toast sau 2 giây.
 * Luôn clear timeout cũ trước khi tạo mới để tránh xung đột.
 * createdby: kxtrien - 02.12.2025
 */
const startTimer = () => {
  if (timeout) clearTimeout(timeout)

  timeout = setTimeout(() => {
    isShow.value = false
  }, 2000)
}

/**
 * Tạm dừng bộ đếm ngược.
 * Sử dụng khi người dùng di chuột (hover) vào Toast.
 * createdby: kxtrien - 02.12.2025
 */
const pauseTimer = () => {
  if (timeout) clearTimeout(timeout)
}
/**
 * Tiếp tục bộ đếm ngược.
 * Sử dụng khi người dùng đưa chuột ra khỏi Toast (mouseleave).
 * createdby: kxtrien - 02.12.2025
 */
const resumeTimer = () => {
  if (isShow.value) {
    startTimer()
  }
}

/**
 * Đóng Toast ngay lập tức khi người dùng bấm nút X.
 * createdby: kxtrien - 02.12.2025
 */
const closeToast = () => {
  isShow.value = false
  if (timeout) clearTimeout(timeout)
}

defineExpose({
  showSuccessToast,
})
</script>

<style scoped>
.ms-toast {
  position: fixed;
  top: 80px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  align-items: center;
  background-color: var(--color-primary);
  border-radius: 4px;
  box-shadow: 0 3px 6px rgba(0, 0, 0, 0.16);
  min-width: 300px;
  height: 50px;
  z-index: 100;
  overflow: hidden;
}

.ms-toast-left-line {
  width: 7px;
  height: 100%;
  background-color: var(--color-primary);
}
.container {
  padding-left: 10px;
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: space-between;
  background-color: #ffffff;
  border-radius: 4px;
  padding-right: 16px;
}
.container .main {
  gap: 8px;
  display: flex;
  align-items: center;
}
.container .main .check {
  color: var(--color-primary);
}

.slide-down-enter-active,
.slide-down-leave-active {
  transition: all 0.4s ease;
}

.slide-down-enter-from,
.slide-down-leave-to {
  opacity: 0;
  transform: translateX(-50%) translateY(-100%);
}

.slide-down-enter-to,
.slide-down-leave-from {
  opacity: 1;
  transform: translateX(-50%) translateY(0);
}
</style>
