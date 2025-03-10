using Microsoft.EntityFrameworkCore;

namespace Payroll_Test_2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Login> Logins { get; set; } // ✅ Ensure Logins table is mapped

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employees"); // ✅ Matches DB table
            modelBuilder.Entity<Login>().ToTable("Logins"); // ✅ Matches DB table
        }
    }
}
