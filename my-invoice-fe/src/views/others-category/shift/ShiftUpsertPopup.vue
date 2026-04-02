<script setup>
import MSAlert from '@/components/ms-alert/MSAlert.vue'
import MSButton from '@/components/ms-button/MSButton.vue'
import MSDialog from '@/components/ms-dialog/MSDialog.vue'
import MSRadio from '@/components/ms-input/MSRadio.vue'
import MSTimeSelect from '@/components/ms-input/MSTimeSelect.vue'
import MSArrowToolTip from '@/components/ms-tooltip/MSArrowToolTip.vue'
import { TriangleAlert, X } from 'lucide-vue-next'
import { onMounted, onUnmounted, reactive, ref, useTemplateRef, watch, watchEffect } from 'vue'

//#region Props & Emit
const props = defineProps(['popupData'])
const emit = defineEmits(['close', 'submit'])
//#end region

//#region State
const isShowAlert = ref(false)
const alertMessage = ref('')
const data = ref(
  props.popupData
    ? { ...props.popupData }
    : {
        shiftCode: '',
        shiftName: '',
        startTime: '',
        endTime: '',
        breakStart: '',
        breakEnd: '',
        workingTime: 0,
        breakTime: 0,
        description: '',
        status: 1,
      },
)
const errors = ref({
  shiftCode: '',
  shiftName: '',
  startTime: '',
  endTime: '',
})
//#end region

//#region Refs
const shiftCodeInput = useTemplateRef('shiftCodeInput')
const shiftNameInput = useTemplateRef('shiftNameInput')
const startTimeInput = useTemplateRef('startTimeInput')
const endTimeInput = useTemplateRef('endTimeInput')
const fieldRefs = {
  shiftCode: shiftCodeInput,
  shiftName: shiftNameInput,
  startTime: startTimeInput,
  endTime: endTimeInput,
}
//#end region

//#region
/**
 * Tự động tính toán giờ làm và giờ nghỉ khi thời gian thay đổi
 * createdby: kxtrien - 02.12.2025
 */
// watch(
//   data,
//   (newVal) => {
//     console.log('changed:', newVal)
//   },
//   { deep: true },
// )

watch(
  [
    () => data.value.startTime,
    () => data.value.endTime,
    () => data.value.breakStart,
    () => data.value.breakEnd,
  ],
  () => {
    const totalShiftHours = calculateDuration(data.value.startTime, data.value.endTime)
    const totalBreakHours = calculateDuration(data.value.breakStart, data.value.breakEnd)

    // Tính giờ làm việc thực tế (= Tổng ca - Giờ nghỉ)
    const realShiftHours = totalShiftHours - totalBreakHours
    data.value.breakTime = totalBreakHours
    data.value.workingTime = realShiftHours
  },
)

/**
 * Format giá trị hiển thị số (Âm thì hiển thị trong ngoặc)
 * @param val Giá trị số
 * createdby: kxtrien - 02.12.2025
 */
const formatDisplayValue = (val) => {
  if (val === '' || val === null || val === undefined) return ''
  if (val < 0) {
    return `(${Math.abs(val)})`
  }
  return val
}

/**
 * Tính khoảng thời gian giữa 2 mốc giờ (theo giờ)
 * @param start Giờ bắt đầu (HH:MM)
 * @param end Giờ kết thúc (HH:MM)
 * createdby: kxtrien - 02.12.2025
 */
const calculateDuration = (start, end) => {
  if (!start || !end) return 0

  const [startH, startM] = start.split(':').map(Number)
  const [endH, endM] = end.split(':').map(Number)

  // Đổi ra tổng phút
  const startTotalMins = startH * 60 + startM
  const endTotalMins = endH * 60 + endM

  let diffMins = endTotalMins - startTotalMins

  //Qua đêm
  if (diffMins < 0) {
    diffMins += 24 * 60
  }
  let hours = diffMins / 60
  return hours
  // return Math.round(hours)
}

/**
 * Validate Mã ca
 * createdby: kxtrien - 02.12.2025
 */
const validateShiftCode = () => {
  if (!data.value.shiftCode?.trim()) {
    errors.value.shiftCode = 'Mã ca không được để trống.'
  } else if (data.value.shiftCode.trim().length > 20) {
    errors.value.shiftCode = 'Mã ca tối đa 20 ký tự.'
  } else {
    errors.value.shiftCode = '' // Hết lỗi
  }
}

/**
 * Validate Tên ca
 * createdby: kxtrien - 02.12.2025
 */
const validateShiftName = () => {
  if (!data.value.shiftName?.trim()) {
    errors.value.shiftName = 'Tên ca không được để trống.'
  } else if (data.value.shiftName.trim().length > 50) {
    errors.value.shiftName = 'Tên ca tối đa 50 ký tự.'
  } else {
    errors.value.shiftName = ''
  }
}

/**
 * Validate Giờ vào
 * createdby: kxtrien - 02.12.2025
 */
const validateStartTime = () => {
  if (!data.value.startTime) {
    errors.value.startTime = 'Giờ vào ca không được để trống.'
  } else {
    errors.value.startTime = ''
  }
}

/**
 * Validate Giờ ra
 * createdby: kxtrien - 02.12.2025
 */
const validateEndTime = () => {
  if (!data.value.endTime) {
    errors.value.endTime = 'Giờ hết ca không được để trống.'
  } else {
    errors.value.endTime = ''
  }
}

/**
 * Xử lý sự kiện phím tắt (Ctrl+S, Ctrl+Shift+S)
 */
const handleGlobalKeydown = (e) => {
  // Kiểm tra Ctrl (Window) hoặc Command (Mac) + phím S
  if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 's') {
    e.preventDefault() // CHẶN trình duyệt mở hộp thoại Save As

    if (e.shiftKey) {
      // Nếu có giữ Shift -> Chạy logic Lưu và Thêm
      handleSaveAndAdd()
    } else {
      // Chỉ ấn Ctrl + S -> Chạy logic Lưu
      handleSave()
    }
  }
}

/**
 * Xử lý lưu dữ liệu
 * createdby: kxtrien - 02.12.2025
 */

const handleSave = () => {
  validateShiftCode()
  validateShiftName()
  validateStartTime()
  validateEndTime()
  // Tìm lỗi đầu tiên
  const firstErrorKey = Object.keys(errors.value).find((key) => errors.value[key] !== '')

  if (firstErrorKey) {
    const message = errors.value[firstErrorKey]

    alertMessage.value = message
    isShowAlert.value = true

    const targetInput = fieldRefs[firstErrorKey]
    if (targetInput && targetInput.value) {
      targetInput.value.focus()
    }

    return
  }

  emit('submit', data.value)
}
const handleSaveAndAdd = () => {
  // Validate dữ liệu (copy logic từ handleSave hoặc tách hàm validate riêng)
  validateShiftCode()
  validateShiftName()
  validateStartTime()
  validateEndTime()

  const firstErrorKey = Object.keys(errors.value).find((key) => errors.value[key] !== '')
  if (firstErrorKey) {
    const message = errors.value[firstErrorKey]
    alertMessage.value = message
    isShowAlert.value = true
    return
  }

  emit('submit', data.value, 'saveAndAdd')
}

/**
 * Đóng popup chính
 */
const handleClosePopup = () => {
  emit('close')
}

/**
 * Đóng alert cảnh báo
 */
const closeAlert = () => {
  isShowAlert.value = false
}

//#region LifeCycle
onMounted(() => {
  shiftCodeInput.value.focus()
  shiftCodeInput.value.select()

  window.addEventListener('keydown', handleGlobalKeydown)
})

onUnmounted(() => {
  window.removeEventListener('keydown', handleGlobalKeydown)
})
//#end region
</script>

<template>
  <MSDialog :title="popupData ? 'Sửa Ca làm việc' : 'Thêm Ca làm việc'" @close="handleClosePopup">
    <!-- Content -->

    <div class="content-row">
      <label for="" class="w-150">Mã ca <span class="required">*</span></label>
      <MSArrowToolTip
        class=""
        style="flex: 1; font-size: 13px"
        :content="errors.shiftCode || 'Mã ca'"
        :position="errors.shiftCode ? 'top' : 'bottom'"
      >
        <input
          type="text"
          tabindex="1"
          ref="shiftCodeInput"
          v-model="data.shiftCode"
          :class="{ 'input-error': errors.shiftCode }"
          @blur="validateShiftCode"
      /></MSArrowToolTip>
    </div>
    <div class="content-row">
      <label for="" class="w-150">Tên ca <span class="required">*</span></label>
      <MSArrowToolTip
        class=""
        style="flex: 1; font-size: 13px"
        :content="errors.shiftName || 'Tên ca'"
        :position="errors.shiftName ? 'top' : 'bottom'"
        ><input
          type="text"
          tabindex="2"
          ref="shiftNameInput"
          v-model="data.shiftName"
          :class="{ 'input-error': errors.shiftName }"
          @blur="validateShiftName"
      /></MSArrowToolTip>
    </div>
    <div class="content-multi-col">
      <div class="content-col">
        <label for="" class="w-150">Giờ vào ca <span class="required">*</span></label>
        <MSArrowToolTip
          class=""
          style="font-size: 13px"
          :content="errors.startTime"
          position="bottom"
        >
          <MSTimeSelect
            v-model="data.startTime"
            ref="startTimeInput"
            tabindex="3"
            class="select"
            :class="{ 'input-error': errors.startTime }"
            @blur="validateStartTime"
        /></MSArrowToolTip>
      </div>
      <div class="content-col">
        <label for="" class="w-175">Giờ hết ca <span class="required">*</span></label>
        <MSArrowToolTip class="" style="font-size: 13px" :content="errors.endTime" position="bottom"
          ><MSTimeSelect
            v-model="data.endTime"
            ref="endTimeInput"
            tabindex="4"
            class="select"
            :class="{ 'input-error': errors.endTime }"
            @blur="validateEndTime"
        /></MSArrowToolTip>
      </div>
    </div>
    <div class="content-multi-col">
      <div class="content-col">
        <label for="" class="w-150">Bắt đầu nghỉ giữa ca</label>
        <MSTimeSelect v-model="data.breakStart" class="select" tabindex="5" />
      </div>
      <div class="content-col">
        <label for="" class="w-175">Kết thúc nghỉ giữa ca</label>
        <MSTimeSelect v-model="data.breakEnd" class="select" tabindex="6" />
      </div>
    </div>
    <div class="content-multi-col">
      <div class="content-col">
        <label for="" class="w-150">Thời gian làm việc (giờ)</label>
        <input
          type="text"
          disabled
          class="disabled"
          :value="formatDisplayValue(data.workingTime)"
          :class="{ 'text-red': data.workingTime < 0 }"
        />
      </div>
      <div class="content-col">
        <label for="" class="w-175">Thời gian nghỉ giữa ca (giờ)</label>
        <input type="text" v-model="data.breakTime" disabled class="disabled" />
      </div>
    </div>
    <div class="content-col textarea">
      <label for="" class="w-150">Mô tả</label>
      <textarea v-model="data.description" name="" id="" tabindex="7"></textarea>
    </div>
    <div class="content-row" v-if="popupData">
      <label for="" class="w-150">Trạng thái</label>
      <div class="status-radio">
        <div class="status-radio-box">
          <MSRadio
            name="status"
            v-model="data.status"
            :value="1"
            :tabindex="data.status === 1 || data.status === null ? 8 : -1"
          />Đang sử dụng
        </div>
        <div class="status-radio-box">
          <MSRadio
            name="status"
            v-model="data.status"
            :value="0"
            :tabindex="data.state === 0 ? 8 : -1"
          />Ngừng sử dụng
        </div>
      </div>
    </div>

    <template #footer>
      <MSButton type="secondary" @click="handleClosePopup" tabindex="11">Hủy</MSButton>
      <MSButton type="secondary" tabindex="10" @click="handleSaveAndAdd"
        ><MSArrowToolTip content="Ctrl + Shift + S" style="">Lưu và thêm</MSArrowToolTip></MSButton
      >
      <MSButton @click="handleSave" tabindex="9"
        ><MSArrowToolTip content="Ctrl + S" style="">Lưu</MSArrowToolTip></MSButton
      >
    </template>
  </MSDialog>

  <MSAlert :is-show="isShowAlert" title="Cảnh báo">
    <template #message>
      <p>{{ alertMessage }}</p>
    </template>
    <template #footer>
      <MSButton @click="closeAlert">Đóng</MSButton>
    </template>
  </MSAlert>
</template>
<style scoped>
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
.w-150 {
  width: 150px;
}
.w-175 {
  width: 175px;
}

.upsert-container {
  background: white;
  width: 680px;
  border-radius: 4px;
  transition: all 0.3s ease-in-out;
  padding: 0 20px;
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
  margin-right: 32px;
  white-space: nowrap;
  cursor: text;
}
.upsert-header .button-wrapper {
  display: flex;
  align-items: center;
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
.upsert-content .required {
  color: red;
}
.content-row {
  display: flex;
  width: 100%;
  align-items: center;
  gap: 1rem;
}
.content-row label {
  font-weight: 500;
}

.content-row input {
  flex: 1;
  border-radius: 4px;
  border: 1px solid #d1d5db;
  background-color: #fff;
  padding: 5px 12px;
}
.content-row input:focus {
  outline: none;
  border-color: var(--color-primary);
}

.content-multi-col {
  display: flex;
  /* gap: 20px; */
  /* background: red; */
  justify-content: space-between;
}
.content-col {
  /* flex: 1; */
  display: flex;
  align-items: center;
  gap: 1rem;
}

.content-col label {
  white-space: nowrap;
  font-weight: 500;
  min-width: 150px;
  flex-shrink: 0;
}

.content-col .select {
  flex: 1;
  width: 122px;

  /* border-radius: 4px;
  border: 1px solid #d1d5db;
  background-color: #fff;
  padding: 5px 12px; */
  min-width: 0;
}
.content-col input {
  flex: 1;
  width: 122px;

  border-radius: 4px;
  border: 1px solid #d1d5db;
  background-color: #fff;
  padding: 5px 12px;
  min-width: 0;
}
input[disabled] {
  background-color: #f3f4f6 !important;
  color: #4b5563;
}
.content-col input:focus {
  outline: none;
}
.content-col.textarea {
  align-items: start;
}
.content-col.textarea textarea {
  width: 100%;
  height: 68px;
  overflow: auto;
  border-radius: 4px;
  border: 1px solid #d5dfe2;
  padding: 6px 10px;
  resize: none;
}
.content-col.textarea textarea:focus {
  outline: none;
  border-color: var(--color-primary);
}
.text-red {
  color: #ef4444 !important;
  font-weight: 500;
}

input[disabled].text-red {
  color: #ef4444 !important;
}
.input-error {
  border-color: #ef4444 !important;
}
.status-radio {
  display: flex;
  align-items: center;
  gap: 16px;
}
.status-radio-box {
  display: flex;
  align-items: center;
  font-weight: 500;
  gap: 8px;
}

.modal__footer__line {
  border-top: 1px solid #d5dfe2;
}
.upsert-footer {
  padding: 12px 0;
  display: flex;
  justify-content: end;
  gap: 8px;
}
.upsert-footer button:focus {
  /* outline: 2px solid var(--color-primary); */
  /* outline-offset: 3px; */
  outline: none;
}

/* Thay thế bằng box-shadow để bo góc mượt mà */
.upsert-footer button:focus-visible {
  /* Cấu trúc: x y blur spread color */
  /* Lớp 1 (2px): Màu trắng để tạo khoảng cách với nút */
  /* Lớp 2 (4px): Màu xanh (hoặc màu theme) làm viền ngoài */
  box-shadow:
    0 0 0 2px #fff,
    0 0 0 4px var(--color-primary);
}
.upsert-footer button:focus-visible :deep(.tooltip-content) {
  visibility: visible;
  opacity: 1;
}
</style>
