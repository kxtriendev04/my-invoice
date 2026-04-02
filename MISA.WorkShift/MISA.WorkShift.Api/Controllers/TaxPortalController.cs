using Microsoft.AspNetCore.Mvc;
using MISA.WorkShift.Core.Entities;
using MISA.WorkShift.Core.Interfaces.Services;
using MISA.WorkShift.Core.DTOs;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace MISA.WorkShift.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TaxPortalController : ControllerBase
    {
        private readonly IInvoiceRegistrationService _registrationService;
        private readonly IInvoiceService _invoiceService;

        public TaxPortalController(IInvoiceRegistrationService registrationService, IInvoiceService invoiceService)
        {
            _registrationService = registrationService;
            _invoiceService = invoiceService;
        }

        /// <summary>
        /// Lấy tất cả tờ khai đang chờ duyệt (Status = 2: Đã gửi)
        /// </summary>
        [HttpGet("registrations/pending")]
        public IActionResult GetPendingRegistrations()
        {
            // Trong thực tế bạn nên viết hàm riêng ở Repo, ở đây tôi dùng GetAll và Filter để demo nhanh
            var all = _registrationService.GetAll();
            var pending = all.Where(x => x.Status == 2).OrderByDescending(x => x.CreatedDate);
            return Ok(pending);
        }

        /// <summary>
        /// Lấy danh sách hóa đơn chờ cấp mã (Dữ liệu thật từ bảng Invoice)
        /// </summary>
        [HttpGet("invoices/pending")]
        public IActionResult GetPendingInvoices()
        {
            var all = _invoiceService.GetAll();
            // Lọc hóa đơn có CqtStatus = 2 (Đang chờ CQT)
            var res = all.Where(x => x.IssueStatus == 2).OrderByDescending(x => x.InvoiceDate);
            return Ok(res);
        }

        /// <summary>
        /// Phê duyệt tờ khai từ phía CQT
        /// </summary>
        [HttpPost("registrations/{id}/approve")]
        public async Task<IActionResult> ApproveRegistration(Guid id)
        {
            try
            {
                var result = await _registrationService.ApproveRegistrationFromCqt(id);
                if (result)
                    return Ok(new { Success = true, Message = "Phê duyệt thành công" });
                
                return NotFound(new { Success = false, Message = "Không tìm thấy tờ khai" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        /// <summary>
        /// CQT Phê duyệt và cấp mã hóa đơn
        /// </summary>
        [HttpPost("invoices/{id}/approve")]
        public async Task<IActionResult> ApproveInvoice(Guid id)
        {
            try
            {
                var result = await _registrationService.ApproveInvoiceFromCqt(id);
                if (result)
                    return Ok(new { Success = true, Message = "Hóa đơn đã được cấp mã CQT" });

                return NotFound(new { Success = false, Message = "Không tìm thấy hóa đơn" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}