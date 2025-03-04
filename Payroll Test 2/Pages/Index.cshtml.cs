using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Payroll_Test_2.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public string? UserName { get; set; }

    public IActionResult OnGet()
    {
        // Check if user is logged in
        if (HttpContext.Session.GetString("UserName") == null)
        {
            return RedirectToPage("/Account/Login"); // Redirect to login page
        }

        // If logged in, get the user name
        UserName = HttpContext.Session.GetString("UserName");
        return Page();
    }
}
