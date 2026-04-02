<script setup>
import { timeSelect } from '@/utils/constants'
import { Clock3 } from 'lucide-vue-next'
import { computed, onBeforeUnmount, onMounted, ref, useAttrs } from 'vue'

//#region Configuration & Props & Expose
defineOptions({
  inheritAttrs: false,
})
const props = defineProps({
  modelValue: {
    type: String,
    default: '',
  },
})
const emit = defineEmits(['update:modelValue', 'blur'])
defineExpose({
  focus: () => {
    inputRef.value?.focus()
  },
})
//#end region

//#region State & Refs
const attrs = useAttrs()
const inputRef = ref(null)
const isOpenList = ref(false)
const containerRef = ref(null)
//#end region

//#region Attributes Logic
const inputAttrs = computed(() => {
  const { class: className, style, ...rest } = attrs
  return rest
})
//#end region

//#region Methods

/**
 * Xử lý khi chọn giờ từ danh sách gợi ý
 * @param time - Chuỗi giờ (VD: "08:30")
 * createdby: kxtrien - 01.12.2025
 */
const handlePickTime = (time) => {
  emit('update:modelValue', time)
  isOpenList.value = false
}

/**
 * Xử lý khi user nhập liệu vào ô input
 * Chỉ giữ lại số, giới hạn 4 ký tự, tự động thêm dấu : sau 2 số đầu
 * @param event
 * createdby: kxtrien - 01.12.2025
 */
const formatTimeInput = (event) => {
  let value = event.target.value
  value = value.replace(/\D/g, '')
  if (value.length > 4) {
    value = value.slice(0, 4)
  }
  if (value.length > 2) {
    // Thêm :
    value = value.slice(0, 2) + ':' + value.slice(2)
  }
  event.target.value = value // Cập nhật giá trị
  emit('update:modelValue', value)
}

/**
 * Bật tắt danh sách gợi ý
 * createdby: kxtrien - 01.12.2025
 */
const handleToggleList = () => {
  isOpenList.value = !isOpenList.value
}

/**
 * Đóng dropdown khi click ra ngoài component
 * @param e
 * createdby: kxtrien - 01.12.2025
 */
const handleClickOutside = (e) => {
  if (!containerRef.value.contains(e.target)) {
    isOpenList.value = false
  }
}

/**
 * Xử lý blur (khi focus ra ngoài input)
 * createdby: kxtrien - 01.12.2025
 */
const handleBlur = () => {
  emit('blur')
}
//#endregion

//#region Lifecycle
onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside)
})
//#endregion
</script>

<template>
  <div class="container" ref="containerRef" :class="$attrs.class" :style="$attrs.style">
    <input
      ref="inputRef"
      type="text"
      v-bind="inputAttrs"
      :value="modelValue"
      @input="formatTimeInput"
      placeholder="HH:MM"
      @blur="handleBlur"
    />
    <Clock3 class="icon" @click="handleToggleList" />
    <div class="option-list" v-show="isOpenList">
      <div
        class="option-item no-select"
        :class="{ active: modelValue == time }"
        v-for="time in timeSelect"
        :key="time"
        @click="handlePickTime(time)"
      >
        {{ time }}
      </div>
    </div>
  </div>
</template>

<style scoped>
.container {
  width: 100%;
  position: relative;
  border: 1px solid #d1d5db;
  border-radius: 4px;
  background-color: #fff;
}
.container:hover {
  border-color: #9ca3af;
}
input {
  width: 100%;
  padding: 5px 12px;
  background-color: transparent;
  border: none;
}
.container:focus-within {
  border-color: var(--color-primary);
}
input:focus {
  outline: none;
}
.container .icon {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  width: 16px;
  height: 16px;
  right: 12px;
  color: #4b5563;
}
.option-list {
  margin-top: 2px;
  border-radius: 4px;
  position: absolute;
  height: 215px;
  width: 100%;
  border: 1px solid #d5dfe2;
  background-color: #fff;
  padding: 12px 13px 7px 12px;
  z-index: 100;
  overflow-y: auto;
}
.option-item {
  border-radius: 3.5px;
  padding: 6px 10px;
  display: flex;
  cursor: pointer;
  justify-content: center;
  font-weight: 400;
}
.option-item:hover {
  background: #efefef;
}
.option-item.active {
  color: white;
  background: var(--color-primary);
}
</style>
