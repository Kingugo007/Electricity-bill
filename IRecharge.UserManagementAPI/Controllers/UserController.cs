using IRecharge.Core.Application.Interface;
using IRecharge.Core.Application.UserServices;
using Microsoft.AspNetCore.Mvc;

namespace IRecharge.UserManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("createUser")]
        public async Task<IActionResult> CreateUser(CreateUserDto model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.CreateUser(model);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet(template: "getAllUsers")]
        public async Task<IActionResult> GetAllUser()
        {
            var result = await _userService.GetAllUsers();
            return StatusCode(result.StatusCode, result);
        }
    }
}
