using IRecharge.Core.Application.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IRecharge.EventListener
{
    public class WalletBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IWalletService _walletService;

        public WalletBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _walletService = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IWalletService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
         
            while(!stoppingToken.IsCancellationRequested)
            {
                return;
                // Keep the background service running until the app stops
               // await Task.Delay(100000, stoppingToken);
            }
        }
    }

}
