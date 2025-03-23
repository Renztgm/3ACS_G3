using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Payroll_Test_2.Pages.EmployeeAccount
{
    public class EmployeeDashboardModel : PageModel
    {
        public string UserName { get; private set; }
        public string UserPosition { get; private set; }
        public string UserDepartment { get; private set; }

        public IActionResult OnGet()
        {
            // Ensure user is logged in
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToPage("/EmployeeLogin");
            }

            // Get user details from session
            UserName = HttpContext.Session.GetString("UserName") ?? "Guest";
            UserPosition = HttpContext.Session.GetString("UserPosition") ?? "Unknown";
            UserDepartment = HttpContext.Session.GetString("UserDepartment") ?? "Unknown";

            return Page();
        }
    }
}