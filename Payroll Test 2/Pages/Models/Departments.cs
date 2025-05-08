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

        [Column("DepartmentName")]
        [StringLength(100)]
        public string DepartmentName { get; set; } = string.Empty;
        [Column("ManagerID")]
        public int? ManagerId { get; set; } // ✅ Already correct

        [Column("Description")]
        public string? Description { get; set; } // ✅ Make nullable if DB allows NULL

        [Column("Performance")]
        public decimal? Performance { get; set; } // ✅ Make nullable if NULLs exist

        [Column("DateCreated")]
        public DateTime? DateCreated { get; set; } // ✅ Make nullable if NULLs exist

        [Column("Budget")]
        public decimal? Budget { get; set; } // ✅ Make nullable if NULLs exist

        [Column("Status")]
        public string? Status { get; set; } // ✅ Make nullable if DB allows NULL

        public ICollection<Employee>? Employees { get; set; } // Nullable navigation if needed
    }

}
