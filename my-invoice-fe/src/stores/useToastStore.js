import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useToastStore = defineStore('toast', () => {
  const isShow = ref(false)
  const message = ref('')
  const type = ref('success')
  const timeout = ref(null) // ✅ FIX

  const showToast = (msg, toastType = 'success') => {
    message.value = msg
    type.value = toastType
    isShow.value = true

    if (timeout.value) clearTimeout(timeout.value)

    timeout.value = setTimeout(() => {
      isShow.value = false
    }, 2000)
  }

  const closeToast = () => {
    isShow.value = false
    if (timeout.value) clearTimeout(timeout.value)
  }

  const success = (msg) => showToast(msg, 'success')
  const error = (msg) => showToast(msg, 'error')
  const warning = (msg) => showToast(msg, 'warning')

  return { isShow, message, type, success, error, warning, closeToast }
})
