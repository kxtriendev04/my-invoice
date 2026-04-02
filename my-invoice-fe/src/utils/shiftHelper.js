/**
 * Hàm chuẩn hóa dữ liệu form Shift trước khi gửi lên API
 * @param formData - Dữ liệu thô từ form
 * createdby: kxtrien - 4/12/2025

 */
export const processShiftPayload = (formData) => {
  // 1. Clone object để tránh tham chiếu ngược
  const payload = { ...formData }

  const DUMMY_DATE = '2000-01-01'

  const formatTime = (timeStr) => {
    if (!timeStr || (typeof timeStr === 'string' && timeStr.trim() === '')) return null

    // Nếu timeStr là chuỗi và chưa có ngày (chưa có chữ 'T'), thì ghép ngày giả vào
    if (typeof timeStr === 'string' && !timeStr.includes('T')) {
      return `${DUMMY_DATE}T${timeStr}:00`
    }
    // Nếu đã là ISO string hoặc định dạng khác hợp lệ thì giữ nguyên
    return timeStr
  }

  // 2. Xử lý các trường thời gian bắt buộc (Start/End)
  if (payload.startTime) payload.startTime = formatTime(payload.startTime)
  if (payload.endTime) payload.endTime = formatTime(payload.endTime)

  // 3. Xử lý các trường thời gian Nullable (BreakStart/End)
  payload.breakStart = formatTime(payload.breakStart)
  payload.breakEnd = formatTime(payload.breakEnd)

  // 4. Xử lý các trường số (Number)
  payload.workingTime = Number(payload.workingTime)
  payload.breakTime = payload.breakTime ? Number(payload.breakTime) : 0
  payload.status = Number(payload.status)

  if (payload.createdDate) {
    payload.createdDate = null
  }
  if (payload.modifiedDate === '') payload.modifiedDate = null
  if (payload.description === '') payload.description = null

  return payload
}
