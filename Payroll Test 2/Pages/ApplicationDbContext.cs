using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Data;
using Payroll_Test_2.Pages.Models;

namespace Payroll_Test_2.Pages
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
        public DbSet<Position> Positions { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Payroll_Test_2.Pages.Models.Payroll> Payroll { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Login>().ToTable("Logins");
            modelBuilder.Entity<Department>().ToTable("Departments");
            modelBuilder.Entity<Position>().ToTable("Positions");
            modelBuilder.Entity<Attendance>().ToTable("Attendance");
            modelBuilder.Entity<Payroll_Test_2.Pages.Models.Payroll>().ToTable("Payroll");
        }
    }
}
