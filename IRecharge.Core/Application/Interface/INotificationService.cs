namespace IRecharge.Core.Application.Interface
{
    public interface INotificationService
    {
        Task SendToken(string userId, string token);
    }
}