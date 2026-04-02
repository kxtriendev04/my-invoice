<template>
  <div class="overlay-upper" v-if="isShow">
    <div class="alert-form">
      <div class="alert-form-content">
        <div class="title-wrapper">
          <h3>
            <TriangleAlert style="color: #ea580c; width: 20px" />
            {{ title }}
          </h3>
          <div class="tooltip-container">
            <MSArrowToolTip v-show="isShowXButton" content="Đóng (ESC)"
              ><X style="color: #4b5563" size="20" @click="onClose"
            /></MSArrowToolTip>
          </div>
        </div>
        <slot name="message"></slot>
      </div>

      <div class="msg-button">
        <slot name="footer"></slot>
      </div>
    </div>
  </div>
</template>

<script setup>
import MSButton from '@/components/ms-button/MSButton.vue'
import { TriangleAlert, X } from 'lucide-vue-next'
import MSArrowToolTip from '../ms-tooltip/MSArrowToolTip.vue'
import { onMounted, onUnmounted } from 'vue'

//#region Props & Emit
const props = defineProps({
  isShow: Boolean,
  title: { type: String, default: 'Cảnh báo!' },
  isShowXButton: { type: Boolean, default: false },
})
const emit = defineEmits(['close'])
//#endregion

//#region Methods
const onClose = () => {
  emit('close')
}

// Hàm xử lý khi ấn phím
const handleKeydown = (e) => {
  if (props.isShow && props.isShowXButton && e.key === 'Escape') {
    onClose()
  }
}
//#endregion

//#region Lifecycle
onMounted(() => {
  window.addEventListener('keydown', handleKeydown)
})

onUnmounted(() => {
  window.removeEventListener('keydown', handleKeydown)
})
//#endregion
</script>

<style scoped>
.overlay-upper {
  position: fixed;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  background-color: rgba(0, 0, 0, 0.5);
  z-index: 25;
  display: flex;
  align-items: center;
  justify-content: center;
}
.alert-form {
  width: 432px;
  min-height: 136px;
  background-color: #fff;
  box-shadow: 0 3px 20px #00000014;
  border-radius: 4px;
  padding: 16px;
}
.alert-form-content {
  display: flex;
  flex-direction: column;
  gap: 16px;
  margin-bottom: 16px;
}
.title-wrapper {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.alert-form-content h3 {
  display: flex;
  gap: 8px;
  align-items: center;
  font-weight: 600;
  color: #111827;
  font-size: 20px;
}
.msg-button {
  display: flex;
  justify-content: end;
}
</style>
