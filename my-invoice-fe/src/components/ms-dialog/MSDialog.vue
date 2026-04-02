<script setup>
import { X } from 'lucide-vue-next'
import MSArrowToolTip from '../ms-tooltip/MSArrowToolTip.vue'
import { onMounted, onUnmounted } from 'vue'
//#region Props & Emit
defineProps({
  title: String,
  width: { type: String, default: '680px' },
})
const emit = defineEmits(['close'])
//#endregion

//#region Handle Global Keydown (ESC)
const handleKeydown = (e) => {
  if (e.key === 'Escape') {
    emit('close')
  }
}

onMounted(() => {
  window.addEventListener('keydown', handleKeydown)
})

onUnmounted(() => {
  window.removeEventListener('keydown', handleKeydown)
})
//#endregion
</script>

<template>
  <div class="overlay">
    <div class="upsert-container" :style="{ width: width }">
      <div class="upsert-header">
        <h1>{{ title }}</h1>
        <div class="button-wrapper">
          <MSArrowToolTip content="Trợ giúp"><div class="icon-popup-help"></div></MSArrowToolTip
          ><MSArrowToolTip content="Đóng (ESC)"
            ><X @click="$emit('close')" class="icon"
          /></MSArrowToolTip>
        </div>
      </div>

      <div class="upsert-content">
        <slot></slot>
      </div>

      <div class="upsert-footer">
        <slot name="footer"></slot>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Copy CSS của Overlay, Header, Footer từ file cũ sang đây */
.overlay {
  position: fixed;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  background-color: rgba(0, 0, 0, 0.2);
  z-index: 20;
  display: flex;
  align-items: center;
  justify-content: center;
}
.upsert-container {
  background: white;
  border-radius: 4px;
  transition: all 0.3s ease-in-out;
  padding: 0 20px;
  display: flex;
  flex-direction: column;
  /* Thêm max-height để tránh popup dài quá màn hình */
  max-height: 90vh;
}
.upsert-header {
  padding: 16px 0;
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.upsert-header h1 {
  font-weight: 700;
  font-size: 24px;
  color: #111827;
}
.button-wrapper {
  display: flex;
  align-items: center;
  gap: 8px;
}
.button-wrapper .icon {
  width: 20px;
  height: 20px;
  cursor: pointer;
  color: #4b5563;
}

.upsert-content {
  padding: 20px 0;
  width: 100%;
  min-height: 322px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.upsert-footer {
  padding: 12px 0;
  display: flex;
  justify-content: end;
  gap: 8px;
}
</style>
