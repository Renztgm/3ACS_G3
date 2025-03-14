using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Payroll_Test_2.Data
{
    [Table("positions")] // Maps to the database table "positions"
    public class Position
    {
        [Key]
        [Column("PositionID")]
        public int PositionId { get; set; }

        [Required]
        [Column("PositionName")]
        [StringLength(100)]
        public string PositionName { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation Property - One Position can have many Employees
        public ICollection<Employee> Employees { get; set; }
    }
}
