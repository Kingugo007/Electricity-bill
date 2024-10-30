namespace IRecharge.Core.Application.WalletServices
{
    public class WalletFundAddedEvent
    {
        public string WalletId { get; set; }
        public decimal Amount { get; set; }
        public string UserId { get; set; }
    }

    public class FundWallet
    {
        public string WalletId { get; set; }
        public decimal Amount { get; set; }
        public string UserId { get; set; }
    }

    public class ProcessPaymentRequest
    {
        public string BillId { get; set; }
        public string UserId { get; set; }
    }

    public class ProcessPaymentResponse
    {
        public string BillId { get; set; }
        public decimal Amount { get; set; }
        public string Token {  get; set; }
    }

    public class CreateWallet
    {
        public string UserId { get; set; }
        public string PhoneNumber { get; set; }
    }
}
