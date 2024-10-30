namespace IRecharge.Core.Application.BillingServices.BillDto
{
    public class CreateBillRequest
    {
        public string UserId { get; set; }
        public string WalletId { get; set; }
        public decimal Amount { get; set; }
    }
    public class BillCreatedEvent
    {
        public string BillId { get; set; }
        public decimal Amount { get; set; }
        public string UserId { get; set;}
    }

    public class PaymentCompletedEvent
    {
        public string BillId { get; set; }
        public decimal Amount { get; set; }
        public string WalletId { get; set;}
        public string UserId { get; set;}
        public string Token {  get; set;}   
    }
}
