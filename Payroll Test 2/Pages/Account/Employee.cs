using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll_Test_2.Data
{
    [Table("employee")] // ✅ Ensures correct table mapping
    public class Employee
    {
        [Key]
        [Column("employee_id")] // ✅ Maps to SQL column
        public int EmployeeId { get; set; } 

        [Column("username")] // ✅ Matches database column name
        public string Username { get; set; }

        [Column("password")] // ⚠️ Ensure column exists, but consider hashing passwords!
        public string Password { get; set; }

        [Column("first_name")] // ✅ Matches database column
        public string FirstName { get; set; }
    }
}
