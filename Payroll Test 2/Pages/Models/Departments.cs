using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Payroll_Test_2.Pages.Models
{
    [Table("departments")] // Maps to the database table "departments"
    public class Department
    {
        [Key]
        [Column("DepartmentID")]
        public int DepartmentId { get; set; }

        [Required]
        [Column("DepartmentName")]
        [StringLength(100)]
        public string DepartmentName { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation Property - One Department can have many Employees
        public ICollection<Employee> Employees { get; set; }
    }
}
