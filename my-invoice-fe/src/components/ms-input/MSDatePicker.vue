<script setup>
import { ref, watch } from 'vue'
import { Calendar } from 'lucide-vue-next'

//#region Props & Emit
const props = defineProps(['modelValue'])
const emit = defineEmits(['update:modelValue'])
//#endregion

//#region State
const displayValue = ref(props.modelValue || '') // (dd/mm/yyyy)
//#endregion

//#region Methods

/**
 * Xử lý format tự động thêm dấu gạch chéo khi người dùng nhập liệu
 * Logic: Chỉ nhận số, tự động chèn '/' sau ký tự thứ 2 và thứ 5.
 * VD: Nhập 20112025 -> 20/11/2025
 * @param e - Sự kiện input từ ô text
 * createdby: kxtrien - 02.12.2025
 */
const onInputText = (e) => {
  let val = e.target.value.replace(/\D/g, '') // Chỉ lấy số
  if (val.length > 2) val = val.substring(0, 2) + '/' + val.substring(2)
  if (val.length > 5) val = val.substring(0, 5) + '/' + val.substring(5)

  displayValue.value = val
  emit('update:modelValue', val)
}

/**
 * Xử lý khi người dùng chọn ngày từ Native Date Picker
 * Logic: Convert từ chuẩn ISO (yyyy-mm-dd) sang format hiển thị (dd/mm/yyyy)
 * @param e - Sự kiện change/input từ input type="date"
 * createdby: kxtrien - 02.12.2025
 */
const onDateSelect = (e) => {
  const isoDate = e.target.value
  if (!isoDate) return

  // Convert: yyyy-mm-dd -> dd/mm/yyyy
  const [y, m, d] = isoDate.split('-')
  const formatted = `${d}/${m}/${y}`

  displayValue.value = formatted
  emit('update:modelValue', formatted)
}
//#endregion

//#region Watchers
/**
 * Theo dõi sự thay đổi từ Props để đồng bộ lại giao diện
 * (Dùng cho trường hợp form cha reset dữ liệu hoặc bind giá trị ban đầu)
 * createdby: kxtrien - 02.12.2025
 */
watch(
  () => props.modelValue,
  (newVal) => {
    if (newVal === '' || newVal === null) {
      displayValue.value = ''
    } else {
      displayValue.value = newVal
    }
  },
)
//#endregion
</script>

<template>
  <div class="ms-datepicker-wrapper">
    <input
      type="text"
      class="custom-input date-display"
      placeholder="dd/mm/yyyy"
      v-model="displayValue"
      @input="onInputText"
      maxlength="10"
    />

    <div class="calendar-trigger">
      <Calendar :size="16" class="icon-calendar" />

      <input type="date" class="hidden-native-input" @input="onDateSelect" />
    </div>
  </div>
</template>

<style scoped>
.ms-datepicker-wrapper {
  position: relative;
  width: 100%;
  display: flex;
  align-items: center;
}

.custom-input {
  width: 100%;
  height: 32px;
  padding: 0 30px 0 8px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  outline: none;
  box-sizing: border-box;
}
.custom-input:focus {
  border-color: #00a65a;
}

.calendar-trigger {
  position: absolute;
  right: 8px;
  top: 50%;
  transform: translateY(-50%);
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
}

.icon-calendar {
  color: #6b7280;
  pointer-events: none;
}

.hidden-native-input {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  opacity: 0;
  cursor: pointer;
  z-index: 10;
}
/* Fix lỗi icon lịch mặc định của Chrome */
.hidden-native-input::-webkit-calendar-picker-indicator {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  margin: 0;
  padding: 0;
  cursor: pointer;
}
</style>
