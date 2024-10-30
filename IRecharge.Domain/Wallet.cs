namespace IRecharge.Domain
{
    public class Wallet
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public decimal Balance { get; set; }
        public string UserId { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;    

    }
}
