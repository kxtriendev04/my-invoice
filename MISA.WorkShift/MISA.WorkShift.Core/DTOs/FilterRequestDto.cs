﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WorkShift.Core.DTOs
{
    /// <summary>
    /// DTO chứa thông tin yêu cầu lấy dữ liệu (phân trang, tìm kiếm, lọc, sắp xếp) từ Client.
    /// createdby: kxtrien - 04.12.2025
    /// </summary>
    public class FilterRequestDto
    {
        /// <summary>
        /// Từ khóa tìm kiếm chung (Search All) trên nhiều cột.
        /// createdby: kxtrien - 04.12.2025
        /// </summary>
        public string? Keyword { get; set; }

        /// Số thứ tự trang hiện tại (Bắt đầu từ 1).
        /// createdby: kxtrien - 04.12.2025
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// Số lượng bản ghi trên một trang (Mặc định 20).
        /// createdby: kxtrien - 04.12.2025
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// Danh sách các tùy chọn sắp xếp (Sort).
        /// createdby: kxtrien - 04.12.2025
        /// </summary>
        public List<SortOption>? SortOptions { get; set; }

        /// <summary>
        /// Bộ lọc nâng cao theo từng cột (Advanced Filter).
        /// Key: Tên thuộc tính (VD: shiftCode), Value: Điều kiện lọc (Operator, Value).
        /// createdby: kxtrien - 04.12.2025
        /// </summary>
        public Dictionary<string, FilterCondition>? Filters { get; set; }
    }

    /// <summary>
    /// Đối tượng chứa thông tin chi tiết của một điều kiện lọc.
    /// createdby: kxtrien - 04.12.2025
    /// </summary>
    public class FilterCondition
    {
        /// <summary>
        /// Toán tử so sánh (Ví dụ: eq, neq, contains, gt, lt, empty...).
        /// createdby: kxtrien - 04.12.2025
        /// </summary>
        public string Operator { get; set; } = string.Empty;

        /// <summary>
        /// Giá trị cần lọc (Có thể là string, int, date...).
        /// createdby: kxtrien - 04.12.2025
        /// </summary>
        public object? Value { get; set; }   
    }
}
