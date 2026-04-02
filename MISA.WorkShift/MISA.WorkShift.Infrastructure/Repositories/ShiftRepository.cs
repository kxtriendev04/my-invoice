﻿using Microsoft.EntityFrameworkCore;
using MISA.WorkShift.Core.DTOs;
using MISA.WorkShift.Core.Entities;
using MISA.WorkShift.Core.Interfaces.Repositories;
using MISA.WorkShift.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MISA.WorkShift.Infrastructure.Repositories
{
    /// <summary>
    /// Repository chuyên biệt để xử lý các thao tác Database cho bảng Ca làm việc (Shift).
    /// Kế thừa các phương thức chung từ BaseRepository.
    /// createdby: kxtrien - 01.12.2025
    /// </summary>
    public class ShiftRepository : BaseRepository<Shift>, IShiftRepository
    {
        /// <summary>
        /// Khởi tạo ShiftRepository với cấu hình kết nối.
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        public ShiftRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Ghi đè danh sách các cột cho phép tìm kiếm chung (Keyword).
        /// Bao gồm: Mã ca, Tên ca, Người tạo.
        /// createdby: kxtrien - 04.12.2025
        /// </summary>
        protected override string[] SearchableColumns => new string[]
        {
            "shift_code",
            "shift_name",
            "created_by"
        };

        /// <summary>
        /// Kiểm tra xem mã ca làm việc đã tồn tại trong hệ thống hay chưa.
        /// createdby: kxtrien - 01.12.2025
        /// </summary>
        /// <param name="shiftCode">Mã ca cần kiểm tra</param>
        /// <returns>True nếu đã tồn tại, False nếu chưa có</returns>
        public bool CheckDuplicateCode(string shiftCode)
        {
            return _dbSet.Any(s => s.ShiftCode == shiftCode);
        }

        /// <summary>
        /// Cập nhật trạng thái hoạt động cho nhiều ca làm việc cùng lúc.
        /// createdby: kxtrien - 05.12.2025
        /// </summary>
        /// <param name="ids">Danh sách ID các ca cần cập nhật</param>
        /// <param name="status">Trạng thái mới (0: Ngừng sử dụng, 1: Đang sử dụng)</param>
        /// <returns>Số bản ghi đã cập nhật thành công</returns>
        public int UpdateStatusMulti(List<Guid> ids, int status)
        {
            var entities = _dbSet.Where(s => ids.Contains(s.ShiftId)).ToList();
            if (entities.Any())
            {
                foreach (var entity in entities)
                {
                    entity.Status = (MISA.WorkShift.Core.Enums.ShiftStatus)status;
                }
                return _context.SaveChanges();
            }
            return 0;
        }
    }
}
