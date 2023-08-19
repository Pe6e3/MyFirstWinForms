using Microsoft.EntityFrameworkCore;
using MyFirstWinForms;

namespace WinForms.DAL.Entities.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies{ get; set; }

    }
}
