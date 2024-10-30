namespace IRecharge.Domain
{
    public class Bill
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Pending";
        public string? UserId { get; set; }
        public string? Token { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
