using IRecharge.Domain;
using Microsoft.EntityFrameworkCore;

namespace IRecharge.Infrastructure.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
    }
}
