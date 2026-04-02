/**
 * Lấy danh sách các toán tử lọc (Filter Operators) dựa trên kiểu dữ liệu cột
 * @param  type - Kiểu dữ liệu của cột ('text' | 'number' | 'date')
 * createdby: kxtrien - 01.12.2025
 */
export const getOperators = (type) => {
  if (type === 'number') {
    return [
      { label: 'Bằng', value: 'eq' },
      { label: 'Khác', value: 'neq' },
      { label: 'Lớn hơn', value: 'gt' },
      { label: 'Lớn hơn hoặc bằng', value: 'gte' },
      { label: 'Nhỏ hơn', value: 'lt' },
      { label: 'Nhỏ hơn hoặc bằng', value: 'lte' },
      { label: 'Trống (Rỗng)', value: 'empty' },
      { label: 'Không trống', value: 'not_empty' },
    ]
  }
  if (type === 'date') {
    return [
      { label: 'Bằng', value: 'eq' },
      { label: 'Khác', value: 'neq' },
      { label: 'Lớn hơn (Sau ngày)', value: 'gt' },
      { label: 'Lớn hơn hoặc bằng', value: 'gte' },
      { label: 'Nhỏ hơn (Trước ngày)', value: 'lt' },
      { label: 'Nhỏ hơn hoặc bằng', value: 'lte' },
      { label: 'Trống', value: 'empty' },
      { label: 'Không trống', value: 'not_empty' },
    ]
  }
  return [
    { label: 'Chứa', value: 'contains' },
    { label: 'Không chứa', value: 'not_contains' },
    { label: 'Bằng', value: 'eq' },
    { label: 'Khác', value: 'neq' },
    { label: 'Bắt đầu bằng', value: 'startsWith' },
    { label: 'Kết thúc bằng', value: 'endsWith' },
    { label: 'Trống (Rỗng)', value: 'empty' },
    { label: 'Không trống', value: 'not_empty' },
  ]
}
