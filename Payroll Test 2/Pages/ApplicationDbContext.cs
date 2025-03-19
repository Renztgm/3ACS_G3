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
        public DbSet<Login> Logins { get; set; } // ✅ Ensure Logins table is mapped
        public DbSet<Department> Departments { get; set; }  // Add this
        public DbSet<Position> Positions { get; set; }  // Add this
        public DbSet<Attendance> Attendances { get; set; }  // ✅ Add Attendance Table

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employees"); // ✅ Matches DB table
            modelBuilder.Entity<Login>().ToTable("Logins"); // ✅ Matches DB table
            modelBuilder.Entity<Department>().ToTable("Departments");  // ✅ Explicitly map
            modelBuilder.Entity<Position>().ToTable("Positions");  // ✅ Explicitly map
            modelBuilder.Entity<Attendance>().ToTable("Attendance");  // ✅ Map Attendance table

        }
    }
}