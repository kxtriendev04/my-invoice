﻿using Microsoft.EntityFrameworkCore;
using MISA.WorkShift.Core.DTOs;
using MISA.WorkShift.Core.Interfaces.Repositories;
using MISA.WorkShift.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace MISA.WorkShift.Infrastructure.Repositories
{
    /// <summary>
    /// Repository cơ sở triển khai các thao tác Database chung sử dụng Dapper.
    /// Triển khai IDisposable để quản lý tài nguyên kết nối Database.
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu Entity</typeparam>
    /// <createdby>kxtrien - 01.12.2025</createdby>
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;
        protected readonly string TableName;
        protected readonly string KeyName;

        /// <summary>
        /// Danh sách các cột cho phép tìm kiếm chung (Keyword) trong bảng.
        /// Các lớp con (VD: ShiftRepository) có thể override để chỉ định cụ thể.
        /// </summary>
        /// <createdby>kxtrien - 04.12.2025</createdby>
        protected virtual string[] SearchableColumns => new string[] { };

        /// <summary>
        /// Khởi tạo Repository và kết nối Database.
        /// Tự động suy diễn tên bảng và khóa chính theo quy tắc PascalCase -> snake_case.
        /// </summary>
        /// <param name="context">EF Core DbContext</param>
        /// <createdby>kxtrien - 01.12.2025</createdby>
        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            
            TableName = typeof(T).Name;

            // Tự động tìm tên thuộc tính Khóa chính từ EF Core Metadata
            var entityType = _context.Model.FindEntityType(typeof(T));
            var primaryKey = entityType?.FindPrimaryKey();
            KeyName = primaryKey?.Properties.Select(x => x.Name).FirstOrDefault() ?? $"{TableName}Id";
        }

        /// <summary>
        /// Lấy toàn bộ danh sách bản ghi.
        /// </summary>
        /// <returns>Danh sách các đối tượng Entity (T).</returns>
        /// <createdby>kxtrien - 01.12.2025</createdby>
        public IEnumerable<T> GetAll()
        {
            return _dbSet.AsNoTracking().ToList();
        }

        /// <summary>
        /// Lấy chi tiết bản ghi theo ID.
        /// </summary>
        /// <param name="id">ID (Guid) của bản ghi.</param>
        /// <returns>Đối tượng Entity (T) tìm thấy, hoặc null nếu không tìm thấy.</returns>
        /// <createdby>kxtrien - 01.12.2025</createdby>
        public T GetById(Guid id)
        {
            return _dbSet.Find(id);
        }

        /// <summary>
        /// Thêm mới bản ghi.
        /// Tự động map các Property của Entity sang cột Database tương ứng.
        /// </summary>
        /// <param name="entity">Đối tượng Entity (T) cần thêm mới.</param>
        /// <returns>Số dòng bị ảnh hưởng (thường là 1 nếu thành công).</returns>
        /// <createdby>kxtrien - 01.12.2025</createdby>
        public int Insert(T entity)
        {
            _dbSet.Add(entity);
            return _context.SaveChanges();
        }

        /// <summary>
        /// Cập nhật thông tin bản ghi.
        /// Bỏ qua khóa chính và các trường CreatedDate, CreatedBy khi cập nhật.
        /// </summary>
        /// <param name="id">ID (Guid) của bản ghi cần cập nhật.</param>
        /// <param name="entity">Đối tượng Entity (T) chứa dữ liệu mới.</param>
        /// <returns>Số dòng bị ảnh hưởng (thường là 1 nếu thành công).</returns>
        /// <createdby>kxtrien - 01.12.2025</createdby>
        public int Update(Guid id, T entity)
        {
            var existing = _dbSet.Find(id);
            if (existing == null) return 0;

            // Cập nhật giá trị từ entity mới vào bản ghi cũ (trừ Created fields)
            _context.Entry(existing).CurrentValues.SetValues(entity);
            
            // Giữ nguyên thông tin tạo bản ghi
            _context.Entry(existing).Property("CreatedDate").IsModified = false;
            _context.Entry(existing).Property("CreatedBy").IsModified = false;

            return _context.SaveChanges();
        }

        /// <summary>
        /// Xóa bản ghi theo ID.
        /// </summary>
        /// <param name="id">ID (Guid) của bản ghi cần xóa.</param>
        /// <returns>Số dòng bị ảnh hưởng (thường là 1 nếu thành công).</returns>
        /// <createdby>kxtrien - 01.12.2025</createdby>
        public int Delete(Guid id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                return _context.SaveChanges();
            }
            return 0;
        }

        /// <summary>
        /// Xóa nhiều bản ghi cùng lúc.
        /// </summary>
        /// <param name="ids">Danh sách ID (List<Guid>) của các bản ghi cần xóa.</param>
        /// <returns>Số bản ghi đã xóa thành công.</returns>
        /// <createdby>kxtrien - 08.12.2025</createdby>
        public int DeleteMulti(List<Guid> ids)
        {
            // Lấy thuộc tính ID động dựa trên KeyName
            var entities = _dbSet.Where(e => ids.Contains(EF.Property<Guid>(e, KeyName))).ToList();

            if (entities.Any())
            {
                _dbSet.RemoveRange(entities);
                return _context.SaveChanges();
            }
            return 0;
        }

        /// <summary>
        /// Kiểm tra ID có tồn tại trong hệ thống không.
        /// </summary>
        /// <param name="id">ID (Guid) cần kiểm tra.</param>
        /// <returns>True nếu bản ghi tồn tại, False nếu không tồn tại.</returns>
        /// <createdby>kxtrien - 01.12.2025</createdby>
        public bool CheckIdExists(Guid id)
        {
            return _dbSet.Any(e => EF.Property<Guid>(e, KeyName) == id);
        }

        /// <summary>
        /// Chuyển đổi snake_case từ FE sang PascalCase của Entity
        /// </summary>
        private string ToPascalCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return string.Concat(str.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                         .Select(s => s.Length > 0 
                            ? char.ToUpper(s[0]) + s.Substring(1) 
                            : s));
        }

        /// <summary>
        /// Lấy danh sách phân trang nâng cao.
        /// </summary>
        public virtual PagingResult<T> GetFilterPaging(string? searchKey, Dictionary<string, FilterCondition>? filters, int limit, int offset, List<SortOption>? sortOptions)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            // 1. Search Logic
            if (!string.IsNullOrEmpty(searchKey) && SearchableColumns.Length > 0)
            {
                Expression<Func<T, bool>> searchExpression = e => false;
                foreach (var col in SearchableColumns)
                {
                    var propertyName = ToPascalCase(col);
                    if (string.IsNullOrEmpty(propertyName)) continue;
                    var parameter = Expression.Parameter(typeof(T), "e");
                    var property = Expression.Property(parameter, propertyName);
                    
                    // Chỉ search trên các cột kiểu chuỗi
                    if (property.Type == typeof(string) && searchKey != null)
                    {
                        var method = typeof(string).GetMethod("Contains", new[] { typeof(string) }) 
                                     ?? throw new InvalidOperationException("Method 'Contains' not found.");
                        var value = Expression.Constant(searchKey, typeof(string));
                        var contains = Expression.Call(property, method, value);
                        var lambda = Expression.Lambda<Func<T, bool>>(contains, parameter);
                        
                        searchExpression = CombineExpressions(searchExpression, lambda, Expression.OrElse);
                    }
                }
                query = query.Where(searchExpression);
            }

            // 2. Filter Logic
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    var propertyName = ToPascalCase(filter.Key);
                    var value = filter.Value.Value?.ToString();
                    if (string.IsNullOrEmpty(value) && filter.Value.Operator != "empty" && filter.Value.Operator != "not_empty") continue;

                    switch (filter.Value.Operator)
                    {
                        case "eq": query = query.Where(e => EF.Property<object>(e, propertyName).ToString() == value); break;
                        case "contains": query = query.Where(e => EF.Property<string>(e, propertyName).Contains(value)); break;
                        // Thêm các toán tử khác tương tự...
                    }
                }
            }

            // 3. Sort Logic
            if (sortOptions != null && sortOptions.Count > 0)
            {
                bool firstSort = true;
                foreach (var sort in sortOptions)
                {
                    var propName = ToPascalCase(sort.ColProp);
                    if (sort.Type?.ToLower() == "desc")
                        query = firstSort ? query.OrderByDescending(e => EF.Property<object>(e, propName)) : ((IOrderedQueryable<T>)query).ThenByDescending(e => EF.Property<object>(e, propName));
                    else
                        query = firstSort ? query.OrderBy(e => EF.Property<object>(e, propName)) : ((IOrderedQueryable<T>)query).ThenBy(e => EF.Property<object>(e, propName));
                    firstSort = false;
                }
            }
            else
            {
                query = query.OrderByDescending(e => EF.Property<DateTime>(e, "CreatedDate"));
            }

            // 4. Execution
            var totalRecords = query.Count();
            var data = query.Skip(offset).Take(limit).ToList();

            return new PagingResult<T>
            {
                Data = data,
                TotalRecords = totalRecords
            };
        }

        private Expression<Func<T, bool>> CombineExpressions(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2, Func<Expression, Expression, BinaryExpression> operation)
        {
            var parameter = Expression.Parameter(typeof(T));
            var left = Expression.Invoke(expr1, parameter);
            var right = Expression.Invoke(expr2, parameter);
            return Expression.Lambda<Func<T, bool>>(operation(left, right), parameter);
        }
    }
}