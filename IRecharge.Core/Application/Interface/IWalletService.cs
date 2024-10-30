using IRecharge.Core.Application.WalletServices;
using IRecharge.Core.Utilities;
using IRecharge.Domain;

namespace IRecharge.Core.Application.Interface
{
    public interface IWalletService
    {
        Task<ApiResponse<ProcessPaymentResponse>> ProcessPayment(ProcessPaymentRequest request);
        Task<ApiResponse<Wallet>> AddFunds(FundWallet request);
        Task<bool> CreateWallet(CreateWallet model);
    }
}