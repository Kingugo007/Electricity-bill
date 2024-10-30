using IRecharge.Core.Application.UserServices;
using IRecharge.Core.Utilities;
using IRecharge.Domain;

namespace IRecharge.Core.Application.Interface
{
    public interface IUserService
    {
        Task<ApiResponse<AppUser>> CreateUser(CreateUserDto model);
        Task<ApiResponse<List<AppUser>>> GetAllUsers();
    }
}