using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Pages.Models;

namespace Payroll_Test_2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Loans> Loans { get; set; }
        public DbSet<Attendance> Attendance { get; set; }
        public DbSet<Deductions> Deductions { get; set; }
        public DbSet<Payroll> Payroll { get; set; }
        public DbSet<Bonus> Bonuses { get; set; }
        public DbSet<LoanHistory> LoanHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Login>().ToTable("Logins");
            modelBuilder.Entity<Department>().ToTable("Departments");
            modelBuilder.Entity<Loans>().ToTable("Loans");
            modelBuilder.Entity<Attendance>().ToTable("Attendance");
            modelBuilder.Entity<Deductions>().ToTable("Deduction");
            modelBuilder.Entity<Payroll>().ToTable("Payroll");
            modelBuilder.Entity<Bonus>().ToTable("Bonuses");
            modelBuilder.Entity<LoanHistory>().ToTable("LoanHistory");


            modelBuilder.Entity<Loans>()
               .Property(l => l.LoanAmount)
               .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Loans>()
                .Property(l => l.PaidLoan)
                .HasColumnType("decimal(18,2)");
        }
    }
}
