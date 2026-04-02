using Microsoft.AspNetCore.Mvc;
using MISA.WorkShift.Core.Entities;
using MISA.WorkShift.Core.Interfaces.Services;

namespace MISA.WorkShift.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : BaseController<User>
    {
        public UsersController(IBaseService<User> baseService) : base(baseService)
        {
        }
    }
}