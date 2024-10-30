using IRecharge.Core.Application.BillingServices;
using IRecharge.Core.Application.Interface;
using IRecharge.Infrastructure;

namespace IRecharge.BillingAPI.Extension
{
    public static class ServiceExtension
    {
        public static void InjectBillingServices(this IServiceCollection services)
        {
            services.AddSingleton<ServiceBusHelper>();
            services.AddScoped<IBillingService, BillingService>();
        }
    }
}
