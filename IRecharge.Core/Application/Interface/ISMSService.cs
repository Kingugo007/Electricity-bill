namespace IRecharge.Core.Application.Interface
{
    public interface ISMSService
    {
        Task<bool> SendSmsAsync(string toPhoneNumber, string message);
    }
}