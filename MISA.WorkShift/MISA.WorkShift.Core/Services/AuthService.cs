using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MISA.WorkShift.Core.DTOs;
using MISA.WorkShift.Core.Entities;
using MISA.WorkShift.Core.Interfaces.Services;
using MISA.WorkShift.Core.Interfaces.Repositories;
using System;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;
using System.Threading.Tasks;

namespace MISA.WorkShift.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponse> Login(LoginRequest request)
        {
            // 1. Tìm user theo Email
            var user = await _authRepository.GetUserByEmail(request.Email);
            if (user == null || !BC.Verify(request.Password, user.PasswordHash))
            {
                throw new Exception("Email hoặc mật khẩu không chính xác.");
            }

            // 2. Kiểm tra xem User có thuộc Công ty có MST này không
            var company = await _authRepository.GetCompanyByTaxCode(request.TaxCode);
            if (company == null)
            {
                throw new Exception("Mã số thuế không tồn tại.");
            }

            var isMember = await _authRepository.IsUserMemberOfCompany(user.UserId, company.CompanyId);
            if (!isMember)
            {
                throw new Exception("Tài khoản không có quyền truy cập vào đơn vị này.");
            }

            // 3. Tạo JWT Token
            var token = GenerateJwtToken(user, company);

            return new AuthResponse
            {
                Token = token,
                User = new UserDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email
                },
                Company = company
            };
        }

        public async Task<bool> Register(RegisterRequest request)
        {
            // 1. Validate tồn tại
            if (await _authRepository.IsTaxCodeRegistered(request.TaxCode))
            {
                throw new Exception("Mã số thuế đã được đăng ký trên hệ thống.");
            }

            if (await _authRepository.IsEmailRegistered(request.Email))
            {
                throw new Exception("Email đã được sử dụng.");
            }
                // 2. Tạo Company mới
                var company = new Company
                {
                    CompanyId = Guid.NewGuid(),
                    CompanyName = request.CompanyName,
                    CompanyTaxCode = request.TaxCode,
                    CreatedDate = DateTime.Now,
                    Status = 1
                };

                // 3. Tạo User mới
                var user = new User
                {
                    UserId = Guid.NewGuid(),
                    Username = request.Email, // Dùng email làm username
                    FullName = request.FullName,
                    Email = request.Email,
                    PasswordHash = BC.HashPassword(request.Password),
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };
                // 4. Tạo liên kết Company - User (Role Admin)
                var companyUser = new CompanyUser
                {
                    CompanyId = company.CompanyId,
                    UserId = user.UserId,
                    RoleId = 1, // Admin
                    CreatedDate = DateTime.Now
                };
                var res = await _authRepository.RegisterUserAndCompany(company, user, companyUser);
                return res > 0;
        }

        private string GenerateJwtToken(User user, Company company)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("TaxCode", company.CompanyTaxCode),
                new Claim("CompanyId", company.CompanyId.ToString()),
                new Claim("FullName", user.FullName)
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}