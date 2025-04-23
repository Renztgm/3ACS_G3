using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Payroll_Test_2.Pages.Models
{
    [Table("employees")] // Maps to the database table "employees"
    public class Employee
    {
        [Key]
        [Column("EmployeeID")]
        public int EmployeeId { get; set; }

        [Required]
        [Column("FirstName")]
        public string FirstName { get; set; }

        [Required]
        [Column("LastName")]
        public string LastName { get; set; }

        [Required, EmailAddress]
        [Column("Email")]
        public string Email { get; set; }

        [Column("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [NotMapped]
        public DateTime? DateOfBirth { get; set; }

        [NotMapped]
        //[Column("Gender")]
        public string Gender { get; set; }

        [NotMapped]
        public string Address { get; set; }

        [Required]
        [ForeignKey("Department")]
        [Column("DepartmentID")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; } // Navigation Property


        //[NotMapped]
        //public int PositionId { get; set; }
        //public Position Position { get; set; } // Navigation Property

        //[Required]
        //[ForeignKey("Role")]
        //[Column("RoleID")]
        //public int RoleId { get; set; }
        //public Role Role { get; set; } // Navigation Property

        [Required]
        [Column("DateHired")]
        public DateTime HireDate { get; set; }

        [Required]
        [Column("Status")]
        public string EmploymentStatus { get; set; } = "Active";

        //[ForeignKey("Supervisor")]
        //[Column("SupervisorID")]

        //public int? SupervisorId { get; set; }
        //public Employee Supervisor { get; set; } // Self-referencing Navigation Property

        //[Column("CreatedAt")]
        //public DateTime CreatedAt { get; set; }

        //[Column("UpdatedAt")]
        //public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<Login> Logins { get; set; } // Navigation Property
    }
}
