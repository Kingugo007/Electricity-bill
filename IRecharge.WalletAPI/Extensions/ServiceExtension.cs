using IRecharge.Core.Application;
using IRecharge.Core.Application.Interface;
using IRecharge.Core.Application.Notification;
using IRecharge.Core.Application.WalletServices;
using IRecharge.Core.Utilities;
using IRecharge.Infrastructure;

namespace IRecharge.WalletAPI.Extensions
{
    public static class ServiceExtension
    {
        public static void InjectWalletServices(this IServiceCollection services)
        {
            services.AddSingleton<ServiceBusHelper>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IHttpClientApiRequestHandler, HttpClientApiRequestHandler>();
            services.AddSingleton<ISMSService, SMSService>();
            services.AddScoped<INotificationService, NotificationService>();
            //services.AddHostedService<WalletBackgroundService>();
        }
    }
}
