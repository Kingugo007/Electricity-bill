

using IRecharge.Core.Application.BillingServices.BillDto;
using IRecharge.Core.Application.Interface;
using IRecharge.Core.Utilities;
using IRecharge.Domain;
using IRecharge.Infrastructure;
using IRecharge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace IRecharge.Core.Application.WalletServices
{
    public class WalletService : IWalletService
    {
        private readonly ServiceBusHelper _serviceBusHelper;
        private readonly ILogger<WalletService> _logger;
        private readonly AppDBContext _appDBContext;
        private readonly IHttpClientApiRequestHandler _apiRequestHandler;
        private readonly ISMSService _smsServive;

        public WalletService(ServiceBusHelper serviceBusHelper, 
            ILogger<WalletService> logger, 
            AppDBContext appDBContext, IHttpClientApiRequestHandler apiRequestHandler, ISMSService smsServive)
        {
            _serviceBusHelper = serviceBusHelper;
            _logger = logger;
            _appDBContext = appDBContext;
            _apiRequestHandler = apiRequestHandler;
            _smsServive = smsServive;
        }

        /// <summary>
        /// Adds funds to a wallet and publishes a "fund-added" event
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public async Task<ApiResponse<Wallet>> AddFunds(FundWallet request)
        {
            ApiResponse<Wallet> response = new ApiResponse<Wallet>();
            try
            {

                //validate amount
                if (request.Amount <= 0)
                {
                    return response.Fail("Invalid amount");
                }

                // Check if walletId exist
                Wallet wallet = await _appDBContext.Wallets.FirstOrDefaultAsync(x => x.Id == request.WalletId && x.UserId == request.UserId);

                if (wallet == null)
                {
                    return response.Fail($"Invalid wallet ID {request.WalletId}");
                }

                wallet.Balance += request.Amount;

                int saveChanges = await _appDBContext.SaveChangesAsync();
                if (saveChanges <= 0)
                    return response.Fail($"An error occurred while funding your wallet, please contact the admin");

                // publish wallet-events
                var fundAddedEvent = new WalletFundAddedEvent { WalletId = request.WalletId, Amount = request.Amount, UserId = request.UserId };
                await _serviceBusHelper.PublishEventAsync("wallet-events", fundAddedEvent);
                return response.Success("success", wallet);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AddFunds {ex.Message}", ex);
                return response.Fail("An error occurred", (int)HttpStatusCode.InternalServerError);
            }
        }


        /// <summary>
        /// The method processes payment and publish event to payment-completed
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ApiResponse<ProcessPaymentResponse>> ProcessPayment(ProcessPaymentRequest request)
        {
            ApiResponse<ProcessPaymentResponse> response = new ApiResponse<ProcessPaymentResponse>();

            try
            {
                // Verify bill
                Bill bill = await _appDBContext.Bills.FirstOrDefaultAsync(b => b.Id == request.BillId && b.Status == "Pending" && b.UserId == request.UserId);

                if (bill == null)
                    return response.Fail("This bill is not found");

                // Get wallet
                Wallet wallet = await _appDBContext.Wallets.FirstOrDefaultAsync(x => x.UserId == request.UserId && x.IsActive);
                if (wallet == null)
                    return response.Fail("Wallet is not found or inactive");

                // Validate balance
                if (wallet.Balance < bill.Amount)
                    return response.Fail("Insufficient funds");

                // Get token from service provider
                var tokens = await _apiRequestHandler.GetAsync<List<ElectricityToken>>("https://632b03111090510116ce8c28.mockapi.io/api/v1/tokens");

                if (!tokens.Any())
                    return response.Fail("Service provider is not available at the moment");

                // set token
                string token = tokens.First().Token;

                bill.Status = "Paid";
                bill.Token = token;
                wallet.Balance -= bill.Amount;

                int saveChanges = await _appDBContext.SaveChangesAsync();

                if (saveChanges <= 0)
                    return response.Fail("An error occurred processing this payment, Please contact the admin");

                // Publish "payment-completed" event
                var paymentCompletedEvent = new PaymentCompletedEvent { BillId = bill.Id, Amount = bill.Amount, UserId = wallet.UserId, WalletId = wallet.Id, Token = token };
                await _serviceBusHelper.PublishEventAsync("payment-completed", paymentCompletedEvent);

                return response.Success("success", new ProcessPaymentResponse { BillId = bill.Id, Amount = bill.Amount, Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError($"ProcessPayment {ex.Message}", ex);
                return response.Fail("An error occurred", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<bool> CreateWallet(CreateWallet model)
        {
            try
            {
                if (model.UserId == null)
                {
                    _logger.LogError($"UserId not found");
                    return false;

                }

                // check if wallet exist
                var user = await _appDBContext.AppUsers.FirstOrDefaultAsync(x => x.Id == model.UserId);

                if (user == null)
                    return false;

                if (user.WalletId != null)
                {
                    _logger.LogError($"This user have an existing wallet {user.Id}");
                    return false;
                }

                // Create wallet
                Wallet wallet = new Wallet
                {
                    UserId = model.UserId,
                    IsActive = true,
                    Balance = 0,
                };

                user.WalletId = wallet.Id;

                await _appDBContext.Wallets.AddAsync(wallet);

                int saveChanges = await _appDBContext.SaveChangesAsync();

                if (saveChanges <= 0)
                    return false;

                // send sms 
                string msg = $"Congratulations, your wallet was successfully created. \n Your wallet ID is {wallet.Id}";

                var smsRes = await _smsServive.SendSmsAsync(model.PhoneNumber, msg);

                if (!smsRes)
                {
                    _logger.LogError($"SMS not send to user {model.PhoneNumber}");
                }

                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError($"CreateWallet {ex.Message}", ex);
                throw;
            }
        }
    }
}
