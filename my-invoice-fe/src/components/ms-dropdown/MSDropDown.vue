<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { ChevronUp, ChevronDown, Check } from 'lucide-vue-next'

//#region Props
const props = defineProps({
  options: {
    type: Array,
    default: () => [10, 20, 30, 50, 100], // Giá trị mặc định
  },
  modelValue: {
    type: [String, Number],
    default: 20,
  },
})
//#endregion

const emit = defineEmits(['update:modelValue'])

//#region State
const isOpen = ref(false)
const containerRef = ref(null)
//#endregion

/**
 * Hàm đảo ngược giá trị đóng/mở
 * createdby: kxtrien - 29.11.2025
 */
const toggleDropdown = () => {
  isOpen.value = !isOpen.value
}

/**
 * Hàm xử lý chọn lựa chọn
 * @param option
 * createdby: kxtrien - 29.11.2025
 */
const selectOption = (option) => {
  emit('update:modelValue', option)
  isOpen.value = false // Chọn xong thì đóng
}

/**
 * Hàm xử lý Click Outside (Click ra ngoài thì đóng)
 * @param event
 * createdby: kxtrien - 29.11.2025
 */
const handleClickOutside = (event) => {
  if (containerRef.value && !containerRef.value.contains(event.target)) {
    isOpen.value = false
  }
}

//#region Lifecycle
onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
//#endregion
</script>

<template>
  <div class="dropdown-container" ref="containerRef">
    <div v-if="isOpen" class="dropdown-menu">
      <div
        v-for="option in options"
        :key="option"
        class="dropdown-item"
        :class="{ active: option === modelValue }"
        @click="selectOption(option)"
      >
        <span class="item-text">{{ option }}</span>

        <Check v-if="option === modelValue" :size="16" class="item-icon" strokeWidth="{1.25}" />
      </div>
    </div>

    <div class="dropdown-trigger" :class="{ 'is-open': isOpen }" @click="toggleDropdown">
      <span class="trigger-text">{{ modelValue }}</span>
      <component :is="isOpen ? ChevronUp : ChevronDown" :size="20" class="trigger-icon" />
    </div>
  </div>
</template>

<style scoped>
/* --- CONTAINER --- */
.dropdown-container {
  position: relative;
  width: 80px; /* Độ rộng cố định giống ảnh */
  font-family: 'Inter', sans-serif;
  user-select: none;
  height: 100%;
  font-size: 13px;
}

/* --- TRIGGER BUTTON --- */
.dropdown-trigger {
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 27px;
  padding: 5px 5px 5px 12px;
  background-color: #fff;
  border: 1px solid #d1d5db; /* Viền xám mặc định */
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
}

.trigger-text {
  color: #111827;
  font-weight: 500;
}

.trigger-icon {
  color: #6b7280;
  pointer-events: none;
}

/* Khi mở: Viền xanh, shadow nhẹ */
.dropdown-trigger.is-open {
  border-color: #00b69b; /* Màu xanh MISA */
}
.dropdown-trigger.is-open .trigger-icon {
  color: #374151; /* Mũi tên đậm hơn chút */
}

/* --- MENU LIST --- */
.dropdown-menu {
  position: absolute;
  bottom: 100%;
  left: 0;
  width: 100%;
  margin-bottom: 4px;

  background-color: #fff;
  border-radius: 4px;
  box-shadow:
    0 0 8px #0000001a,
    0 8px 16px #0000001a;
  z-index: 100;
}

/* --- DROPDOWN ITEM --- */
.dropdown-item {
  display: flex;
  align-items: center;
  justify-content: space-between; /* Đẩy chữ sang trái, icon sang phải */
  padding: 12px 12px;
  color: #1f2937;
  cursor: pointer;
  transition: background-color 0.1s;
  height: 28px;
  line-height: 28px;
}

.dropdown-item:hover {
  background-color: #f3f4f6; /* Hover màu xám nhạt */
}

/* Item đang chọn (Active) */
.dropdown-item.active {
  background-color: var(--color-light-primary);
  color: rgb(0, 155, 113);
}

.item-icon {
  color: rgb(0, 155, 113);
}
</style>
