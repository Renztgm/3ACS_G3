using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Data;

namespace Payroll_Test_2.Pages.EmployeeAccount
{

}
public class EmployeeLoginModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EmployeeLoginModel(ApplicationDbContext context)
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
            return RedirectToPage("/EmployeeAccount/EmployeeDashboard"); // ✅ Redirect if already logged in
        }
        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = _context.Logins
            .Include(l => l.Employee)
            //.ThenInclude(e => e.Position) // ✅ Include Position details
            .Include(l => l.Employee.Department) // ✅ Include Department details
            .FirstOrDefault(l => l.Username == Credential.Username
                              && l.PasswordHash == Credential.Password); // ⚠ Plain-text check (not secure)

        if (user == null)
        {
            ErrorMessage = "Invalid username or password.";
            return Page();
        }

        //if (user.Employee.RoleId != 3)
        //{
        //    ErrorMessage = "Access denied. Only employees can log in.";
        //    return Page();
        //}

        // ✅ Update LastLogin timestamp
        user.LastLogin = DateTime.UtcNow;
        _context.SaveChanges();

        // ✅ Store User Info in Session
        HttpContext.Session.SetInt32("EmployeeID", user.Employee.EmployeeId);
        HttpContext.Session.SetString("UserName", user.Employee.FirstName);
        //HttpContext.Session.SetString("UserPosition", user.Employee.Position?.PositionName ?? "N/A");
        HttpContext.Session.SetString("UserDepartment", user.Employee.Department?.DepartmentName ?? "N/A");

        return RedirectToPage("/EmployeeAccount/EmployeeDashboard"); // ✅ Redirect after successful login
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
