using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Data;
using System.Diagnostics;

namespace Payroll_Test_2.Pages;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ApplicationDbContext _context;

    // Single constructor with both dependencies
    public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public string? UserName { get; set; }
    public int EmployeeCount { get; set; } // Property to store employee count  
    public decimal TotalNetSalary { get; set; }

    public decimal TotalSSS { get; set; }
    public decimal TotalPhilhealth { get; set; }
    public decimal TotalPagibig { get; set; }
    public decimal TotalTIN { get; set; }
    public List<DailySalaryForecast> SalaryForecast { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        // Check if user is logged in
        if (HttpContext.Session.GetString("UserName") == null)
        {
            _logger.LogInformation("User not logged in, redirecting to login page");
            return RedirectToPage("/Account/Login");
        }

        try
        {
            // Get the user name
            UserName = HttpContext.Session.GetString("UserName");

            // Get data from database
            TotalNetSalary = await _context.Payroll.SumAsync(e => e.NetSalary ?? 0m);
            EmployeeCount = await _context.Employees.CountAsync();
            // Get contribution totals
            TotalSSS = await _context.Payroll.SumAsync(p => p.SSS ?? 0m);
            TotalPhilhealth = await _context.Payroll.SumAsync(p => p.Philhealth ?? 0m);
            TotalPagibig = await _context.Payroll.SumAsync(p => p.Pagibig ?? 0m);
            TotalTIN = await _context.Payroll.SumAsync(p => p.TIN ?? 0m);

            // Log successful data retrieval
            _logger.LogInformation($"Dashboard loaded: Employee Count: {EmployeeCount}, Total Net Salary: {TotalNetSalary}");

            return Page();
        }
        catch (Exception ex)
        {
            // Log any errors that occur during database operations
            _logger.LogError(ex, "Error occurred while loading dashboard data");
            TempData["ErrorMessage"] = "An error occurred while loading the dashboard. Please try again later.";
            return Page();
        }
    }

    // Class to represent daily salary forecast data
    public class DailySalaryForecast
    {
        public DateTime Date { get; set; }
        public decimal NetSalary { get; set; }
    }
}