using IRecharge.Core.Application.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Vonage;
using Vonage.Messaging;
using Vonage.Request;


namespace IRecharge.Core.Utilities
{
    public class SMSService : ISMSService
    {
        private readonly ILogger<SMSService> _logger;

        public SMSService(IConfiguration configuration, ILogger<SMSService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SendSmsAsync(string toPhoneNumber, string message)
        {
            try
            {

                toPhoneNumber = FormatPhoneNumber(toPhoneNumber);

                var credentials = Credentials.FromApiKeyAndSecret("eebca55a", "PxObc42AuKKfGSBA");
                var client = new VonageClient(credentials);

                var response = await client.SmsClient.SendAnSmsAsync(new SendSmsRequest()
                {
                    To = $"{toPhoneNumber}",
                    From = "IRecharge",
                    Text = $"{message}"
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"SMSService {ex.Message}", ex);
                throw;
            }

        }

        private string FormatPhoneNumber(string phoneNumber)
        {
            if (phoneNumber.StartsWith("0"))
            {
                phoneNumber = "+234" + phoneNumber.Substring(1);
            }
            else if (!phoneNumber.StartsWith("+"))
            {
                throw new FormatException("Phone number must include the country code.");
            }

            return phoneNumber;
        }

    }
}
