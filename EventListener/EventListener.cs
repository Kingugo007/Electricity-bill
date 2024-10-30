using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRecharge.Infrastructure
{
    public class WalletBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public WalletBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>() as WalletService;

                if (walletService != null)
                {
                    walletService.StartListening();
                }
            }

            // Keep the background service running until the app stops
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }

}
