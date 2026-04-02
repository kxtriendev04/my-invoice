using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WorkShift.Core.MISAAttributes
{
    /// <summary>
    /// Attribute dùng để đánh dấu và ánh xạ tên cột trong Database tương ứng với Property của Entity.
    /// Giúp BaseRepository tự động map dữ liệu chính xác kể cả khi tên Property (CamelCase) khác tên Cột (Snake_case).
    /// createdby: kxtrien - 04.12.2025
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnNameAttribute : Attribute
    {
        /// <summary>
        /// Tên cột thực tế trong bảng Database.
        /// createdby: kxtrien - 04.12.2025
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Khởi tạo Attribute với tên cột cụ thể.
        /// createdby: kxtrien - 04.12.2025
        /// </summary>
        /// <param name="name">Tên cột trong Database (Ví dụ: shift_code)</param>
        public ColumnNameAttribute(string name)
        {
            Name = name;
        }
    }
}
