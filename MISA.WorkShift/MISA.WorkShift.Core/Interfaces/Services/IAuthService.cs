using MISA.WorkShift.Core.DTOs;
using System.Threading.Tasks;

namespace MISA.WorkShift.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(LoginRequest request);
        Task<bool> Register(RegisterRequest request);
    }
}