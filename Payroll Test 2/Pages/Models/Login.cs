using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll_Test_2.Pages.Models
{
    [Table("Logins")] // ✅ Match table name exactly
    public class Login
    {
        [Key]
        [Column("LoginID")]
        public int LoginId { get; set; }

        [Required]
        [Column("EmployeeID")]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }

        [Required]
        [Column("Username")]
        public string Username { get; set; }

        [Required]
        [Column("Password")]
        public string PasswordHash { get; set; }

        [Column("LastLogin")]
        public DateTime? LastLogin { get; set; }

        [Column("FailedAttempts")]
        public int FailedAttempts { get; set; }

        [Column("IsLocked")]
        public bool IsLocked { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
