using Microsoft.EntityFrameworkCore;

namespace TransactionApp.Models
{
    public class TransactionDbContext: DbContext
    {
        public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options)
        {

        }

        public DbSet<TransactionModel> Transaction { get; set; }
    }
}
