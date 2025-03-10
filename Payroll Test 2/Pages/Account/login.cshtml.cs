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
                    .Include(l => l.Employee) // ✅ Load Employee details
                    .FirstOrDefault(l => l.Username == Credential.Username
                                      && l.PasswordHash == Credential.Password);

                if (user != null)
                {
                    // ✅ Update LastLogin timestamp
                    user.LastLogin = DateTime.UtcNow;
                    _context.SaveChanges();

                    // ✅ Store username in session
                    HttpContext.Session.SetString("UserName", user.Employee.FirstName);
                    return RedirectToPage("/Index"); // Redirect after login
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password");
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
