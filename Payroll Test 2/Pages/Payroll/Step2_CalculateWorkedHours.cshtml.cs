using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Payroll_Test_2.Pages.Data;
using Payroll_Test_2.Pages.Models;

public class Step2_CalculateWorkedHoursModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public Step2_CalculateWorkedHoursModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public DateTime StartDate { get; set; }

    [BindProperty]
    public DateTime EndDate { get; set; }

    public List<EmployeeWorkSummary> EmployeeWorkData { get; set; }

    public void OnGet(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;

        // Get all employees
        var employeeData = _context.Employees.ToList();

        var attendanceData = _context.Attendance
            .Where(a => a.Date != null && a.Date >= startDate.Date && a.Date <= endDate.Date)
            .ToList();

        EmployeeWorkData = employeeData.Select(employee => new EmployeeWorkSummary
        {
            EmployeeID = employee.EmployeeId,
            FullName = employee.FirstName + " " + employee.LastName,
            WorkedHours = attendanceData
                .Where(a => a.EmployeeID == employee.EmployeeId)
                .Sum(a =>
                {
                    // Safely check if CheckInTime and CheckOutTime are not null before accessing them
                    if (a.CheckInTime.HasValue && a.CheckOutTime.HasValue)
                    {
                        return (decimal)(a.CheckOutTime.Value - a.CheckInTime.Value).TotalHours;
                    }
                    return 0; // If either is null, return 0
                })
        }).ToList();
    }

    public IActionResult OnPost()
    {
        return RedirectToPage("Step3_Overtime", new { startDate = StartDate, endDate = EndDate });
    }
}

public class EmployeeWorkSummary
{
    public int EmployeeID { get; set; }
    public string FullName { get; set; }
    public decimal WorkedHours { get; set; }
}
