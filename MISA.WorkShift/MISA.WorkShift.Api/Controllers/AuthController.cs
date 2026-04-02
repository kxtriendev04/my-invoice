using Microsoft.AspNetCore.Mvc;
using MISA.WorkShift.Core.DTOs;
using MISA.WorkShift.Core.Enums;
using MISA.WorkShift.Core.Interfaces.Services;
using MISA.WorkShift.Core.Constants;
using System;
using System.Threading.Tasks;

namespace MISA.WorkShift.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _authService.Login(request);
                return Ok(BaseResponse<AuthResponse>.SuccessResponse(result, "Đăng nhập thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(BaseResponse<string>.ErrorResponse(MisaCode.ValidateError, ex.Message));
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _authService.Register(request);
                return StatusCode(201, BaseResponse<bool>.CreatedResponse(result, "Đăng ký tài khoản thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(BaseResponse<string>.ErrorResponse(MisaCode.ValidateError, ex.Message));
            }
        }
    }
}