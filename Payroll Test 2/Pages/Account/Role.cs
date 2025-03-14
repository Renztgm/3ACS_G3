using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Payroll_Test_2.Data
{
    [Table("role_permission")] // Maps to the database table "role_permission"
    public class Role
    {
        [Key]
        [Column("RoleID")]
        public int RoleId { get; set; }

        [Required]
        [Column("RoleName")]
        [StringLength(100)]
        public string RoleName { get; set; }

        [Column("Description")]
        [StringLength(255)]
        public string Description { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation Property - Can be used if you link roles to users
        public ICollection<Employee> Employees { get; set; }
    }
}
