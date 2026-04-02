using MISA.WorkShift.Core.Entities;

namespace MISA.WorkShift.Core.DTOs
{
    public class LoginRequest
    {
        public string TaxCode { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string CompanyName { get; set; }
        public string TaxCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AuthResponse
    {
        public string Token { get; set; }
        public UserDto User { get; set; }
        public Company Company { get; set; }
    }

    public class UserDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}