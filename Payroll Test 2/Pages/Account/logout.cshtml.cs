using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Payroll_Test_2.Pages.Account
{
    public class logoutModel : PageModel
    {
        public IActionResult OnPost()
        {
            HttpContext.Session.Clear(); // Clear the session
            return RedirectToPage("/Account/Login"); // Redirect to login page
        }
    }
}
