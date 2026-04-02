using MISA.WorkShift.Core.Entities;
using MISA.WorkShift.Core.Interfaces.Repositories;
using MISA.WorkShift.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;

namespace MISA.WorkShift.Infrastructure.Repositories
{
    public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Ghi đè các cột cho phép tìm kiếm nhanh trên bảng Hóa đơn.
        /// </summary>
        protected override string[] SearchableColumns => new string[]
        {
            "invoice_number",
            "seller_name",
            "buyer_name",
            "buyer_tax_code"
        };

        /// <summary>
        /// Ví dụ về một phương thức đặc thù: Tự động sinh số hóa đơn tiếp theo.
        /// </summary>
        public string GetNextInvoiceNumber()
        {
            var lastInvoice = _dbSet.OrderByDescending(i => i.InvoiceNumber).FirstOrDefault();
            if (lastInvoice == null || string.IsNullOrEmpty(lastInvoice.InvoiceNumber))
            {
                return "00000001";
            }

            if (int.TryParse(lastInvoice.InvoiceNumber, out int lastNum))
            {
                return (lastNum + 1).ToString().PadLeft(8, '0');
            }
            return "00000001";
        }

        /// <summary>
        /// Lưu hóa đơn và chi tiết trong một giao dịch.
        /// </summary>
        public int InsertInvoiceMasterDetail(Invoice invoice, List<InvoiceDetail> details)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // Lưu Header
                _context.Invoices.Add(invoice);
                
                // Lưu Details
                if (details != null && details.Any())
                {
                    _context.InvoiceDetails.AddRange(details);
                }

                var res = _context.SaveChanges();
                transaction.Commit();
                return res;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public List<InvoiceDetail> GetDetailsByInvoiceId(Guid invoiceId)
        {
            return _context.InvoiceDetails.Where(d => d.InvoiceId == invoiceId).AsNoTracking().ToList();
        }

        public int UpdateInvoiceMasterDetail(Invoice invoice, List<InvoiceDetail> details)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // Update header
                var existing = _context.Invoices.Find(invoice.InvoiceId);
                if (existing == null) throw new Exception("Invoice not found");
                _context.Entry(existing).CurrentValues.SetValues(invoice);
                _context.Entry(existing).Property("CreatedDate").IsModified = false;
                _context.Entry(existing).Property("CreatedBy").IsModified = false;

                // Handle details: simple strategy - delete existing details and insert new ones
                var existingDetails = _context.InvoiceDetails.Where(d => d.InvoiceId == invoice.InvoiceId).ToList();
                if (existingDetails.Any()) _context.InvoiceDetails.RemoveRange(existingDetails);

                if (details != null && details.Any())
                {
                    foreach (var d in details)
                    {
                        d.InvoiceId = invoice.InvoiceId;
                        if (d.DetailId == Guid.Empty) d.DetailId = Guid.NewGuid();
                    }
                    _context.InvoiceDetails.AddRange(details);
                }

                var res = _context.SaveChanges();
                transaction.Commit();
                return res;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Kiểm tra tồn tại hóa đơn theo series trong một công ty
        /// </summary>
        public bool ExistsBySeries(string series, Guid companyId)
        {
            if (string.IsNullOrEmpty(series)) return false;
            return _context.Invoices.Any(i => i.CompanyId == companyId && i.Series == series);
        }
    }
}