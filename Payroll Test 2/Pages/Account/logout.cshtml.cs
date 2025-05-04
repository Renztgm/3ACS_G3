using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Payroll_Test_2.Pages.Account
{
    public class logoutModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            HttpContext.Session.Clear(); // Optional: clear session data
            return RedirectToPage("/Account/Login");
        }
        public async Task<IActionResult> OnPost()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            HttpContext.Session.Clear(); // Clear the session
            return RedirectToPage("/Account/Login"); // Redirect to login page
        }

    }
}
