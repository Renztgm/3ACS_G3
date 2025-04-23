using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Pages.Data;

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
                return RedirectToPage("/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _context.Logins
                .FirstOrDefaultAsync(l => l.Username == Credential.Username && l.PasswordHash == Credential.Password);

            if (user != null)
            {
                HttpContext.Session.SetString("UserName", user.Username);
                return RedirectToPage("/Index");
            }

            ErrorMessage = "Invalid username or password.";
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
