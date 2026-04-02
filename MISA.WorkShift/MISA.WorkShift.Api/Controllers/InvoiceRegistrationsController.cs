using Microsoft.AspNetCore.Mvc;
using MISA.WorkShift.Core.DTOs;
using MISA.WorkShift.Core.Entities;
using MISA.WorkShift.Core.Interfaces.Services;

namespace MISA.WorkShift.Api.Controllers
{
    /// <summary>
    /// Controller xử lý tờ khai đăng ký sử dụng hóa đơn.
    /// </summary>
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class InvoiceRegistrationsController : BaseController<InvoiceRegistration>
    {
        private readonly IInvoiceRegistrationService _registrationService;
        public InvoiceRegistrationsController(IInvoiceRegistrationService registrationService) : base(registrationService)
        {
            _registrationService = registrationService;
        }

        
        [HttpGet("by-company")]
        public IActionResult GetByCompany([FromQuery] Guid? companyId = null)
        {
            try
            {
                if (companyId == null)
                {
                    var companyIdClaim = User?.Claims?.FirstOrDefault(c => c.Type == "CompanyId");
                    if (companyIdClaim == null)
                        return Unauthorized(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.ValidateError, "CompanyId claim not found."));

                    if (!Guid.TryParse(companyIdClaim.Value, out var parsed))
                        return BadRequest(BaseResponse<string>.ErrorResponse(MISA.WorkShift.Core.Enums.MisaCode.ValidateError, "Invalid CompanyId claim."));

                    companyId = parsed;
                }

                var data = _registrationService.GetByCompanyId(companyId.Value);
                return Ok(BaseResponse<IEnumerable<InvoiceRegistration>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("save-full")]
        public async Task<IActionResult> SaveFullRegistration([FromBody] RegistrationSaveDto dto)
        {
            var result = await _registrationService.SaveFullRegistration(dto);
            return Ok(BaseResponse<string>.SuccessResponse(result));
        }
    }
}