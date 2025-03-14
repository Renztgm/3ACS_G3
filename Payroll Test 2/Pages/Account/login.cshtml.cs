using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Data;

namespace Payroll_Test_2.Pages.Account
{
    public class loginModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public loginModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Credential Credential { get; set; }
        public string ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                return RedirectToPage("/Index"); // ✅ Redirect if already logged in
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                var user = _context.Logins
                    .Include(l => l.Employee)
                    .ThenInclude(e => e.Position)  // ✅ Include Position details
                    .Include(l => l.Employee.Department)  // ✅ Include Department details
                    .FirstOrDefault(l => l.Username == Credential.Username
                                      && l.PasswordHash == Credential.Password);

                if (user != null)
                {
                    // ✅ Update LastLogin timestamp
                    user.LastLogin = DateTime.UtcNow;
                    _context.SaveChanges();

                    // ✅ Store User Info in Session
                    HttpContext.Session.SetString("UserName", user.Employee.FirstName);
                    HttpContext.Session.SetString("UserPosition", user.Employee.Position?.PositionName ?? "N/A");
                    HttpContext.Session.SetString("UserDepartment", user.Employee.Department?.DepartmentName ?? "N/A");

                    return RedirectToPage("/Index"); // Redirect after login
                }
                else
                {
                    ErrorMessage = "Invalid username or password.";
                }
            }
            return Page();
        }
    }

    public class Credential
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
