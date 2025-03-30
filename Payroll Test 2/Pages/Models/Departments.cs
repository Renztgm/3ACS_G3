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

        [Required]
        [Column("ManagerID")]
        public int ManagerId { get; set; }

        // Navigation Property - One Department can have many Employees
        public ICollection<Employee> Employees { get; set; }
    }
}
