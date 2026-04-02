/**
 * Hàm chuẩn hóa dữ liệu date trước khi hiện thị ra màn hình
 * @param dateTimeStr - Dữ liệu thô từ form
 * createdby: kxtrien - 4/12/2025

 */
export const formatDateString = (dateTimeStr) => {
  if (dateTimeStr instanceof Date) {
    dateTimeStr = dateTimeStr.toISOString()
  }
  if (!dateTimeStr) return ''
  const dateOnly = dateTimeStr.substring(0, 10)
  const parts = dateOnly.split('-')
  if (parts.length !== 3) return dateTimeStr
  const year = parts[0]
  const month = parts[1]
  const day = parts[2]
  return `${day}/${month}/${year}`
}
