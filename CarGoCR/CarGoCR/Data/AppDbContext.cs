using Microsoft.EntityFrameworkCore;
using CarGoCR.Models;

namespace CarGoCR.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        public DbSet<Cliente> Clientes { get; set; }
    }
}
