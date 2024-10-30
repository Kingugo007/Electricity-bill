using IRecharge.Core.Application.Interface;
using IRecharge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IRecharge.Core.Application.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly ISMSService _smsService;
        private readonly ILogger<NotificationService> _logger;
        private readonly AppDBContext _appDBContext;

        public NotificationService(ISMSService smsService, ILogger<NotificationService> logger, AppDBContext appDBContext)
        {
            _smsService = smsService;
            _logger = logger;
            _appDBContext = appDBContext;
        }

        public async Task SendToken(string userId, string token)
        {
            try
            {
                var user = await _appDBContext.AppUsers.FirstOrDefaultAsync(x => x.Id == userId);
                if (user == null)
                    return;

                // SMS 
                string message = $"You have successfully purchase your electricity token. \n Token - {token}";
                await _smsService.SendSmsAsync(user.PhoneNumber, message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SendToken {ex.Message}", ex);
                throw;
            }


        }
    }
}
