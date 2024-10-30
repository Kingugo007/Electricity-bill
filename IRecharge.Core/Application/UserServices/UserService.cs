using IRecharge.Core.Application.Interface;
using IRecharge.Core.Utilities;
using IRecharge.Domain;
using IRecharge.Infrastructure;
using IRecharge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace IRecharge.Core.Application.UserServices
{
    public class UserService : IUserService
    {
        private readonly ServiceBusHelper _serviceBusHelper;
        private readonly AppDBContext _appDBContext;
        private readonly ILogger<UserService> _logger;

        public UserService(ServiceBusHelper serviceBusHelper, AppDBContext appDBContext, ILogger<UserService> logger)
        {
            _serviceBusHelper = serviceBusHelper;
            _appDBContext = appDBContext;
            _logger = logger;
        }

        /// <summary>
        /// This method creates users and publish a user-event
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ApiResponse<AppUser>> CreateUser(CreateUserDto model)
        {
            ApiResponse<AppUser> response = new ApiResponse<AppUser>();
            try
            {
                // Validate phone number 
                if (model.PhoneNumber.Length != 11)
                    return response.Fail("Invalid phone number");

                // check user
                var checkUser = await _appDBContext.AppUsers.FirstOrDefaultAsync(x => x.PhoneNumber == model.PhoneNumber);

                if (checkUser != null)
                    return response.Fail($"User with this phone number {model.PhoneNumber} exist");

                // Add user
                AppUser user = new AppUser()
                {
                    IsActive = true,
                    PhoneNumber = model.PhoneNumber,
                    Name = model.Name,
                    Email = model.Email,
                };

                await _appDBContext.AppUsers.AddAsync(user);

                int saveChanges = await _appDBContext.SaveChangesAsync();

                if (saveChanges <= 0)
                    return response.Fail($"Unable to save user at the moment, please try again");

                await _serviceBusHelper.PublishEventAsync("user-events", user);
                return response.Success("success", user, (int)HttpStatusCode.Created);

            }
            catch (Exception ex)
            {
                _logger.LogError($"CreateUser {ex.Message}", ex);
                return response.Fail("An error occurred", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<List<AppUser>>> GetAllUsers()
        {
            ApiResponse<List<AppUser>> response = new ApiResponse<List<AppUser>>();

            try
            {
                var users = await _appDBContext.AppUsers.ToListAsync();
                return response.Success("success", users);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetAllUsers {ex.Message}", ex);
                return response.Fail("An error occurred", (int)HttpStatusCode.InternalServerError);
            }

        }
    }
}
