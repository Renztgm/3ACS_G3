using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Pages.Data;
using Payroll_Test_2.Pages.Helpers;
using Payroll_Test_2.Pages.Models;
using System;
using System.Text.Json;

public class Step2_CalculateWorkedHoursModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public Step2_CalculateWorkedHoursModel(ApplicationDbContext context)
    {
        _context = context; 
    }

    [BindProperty(SupportsGet = true)] 
    public DateTime StartDate { get; set; }

    [BindProperty(SupportsGet = true)] 
    public DateTime EndDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Cycle { get; set; }
    public List<EmployeeAttendanceSummary> EmployeeAttendanceData { get; set; } = new();
    public string ErrorMessage { get; set; }

    public class EmployeeAttendanceSummary
    {
        public int EmployeeID { get; set; }
        public string FullName { get; set; }
        public List<Attendance> AttendanceRecords { get; set; }
        public double TotalWorkedHours { get; set; }

        // ✅ Add these
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Cycle { get; set; }  // e.g. "Weekly", "Monthly"
    }

    public async Task OnGetAsync()
    {
        var attendanceData = await _context.Attendance
            .Include(a => a.Employee)
            .Where(a => a.CheckInTime.HasValue && a.CheckOutTime.HasValue &&
                        a.CheckInTime.Value.Date >= StartDate.Date &&
                        a.CheckInTime.Value.Date <= EndDate.Date)
            .ToListAsync();

        EmployeeAttendanceData = attendanceData
            .GroupBy(a => a.EmployeeID)
            .Select(group => new EmployeeAttendanceSummary
            {
                EmployeeID = group.Key,
                FullName = $"{group.First().Employee.FirstName} {group.First().Employee.LastName}",
                AttendanceRecords = group.ToList(),
                TotalWorkedHours = group.Sum(a =>
                {
                    var worked = (a.CheckOutTime.Value - a.CheckInTime.Value).TotalHours;
                    var lunch = (a.LunchStartTime.HasValue && a.LunchEndTime.HasValue)
                        ? (a.LunchEndTime.Value - a.LunchStartTime.Value).TotalHours
                        : 0;
                    return worked - lunch;
                }),
                StartDate = StartDate,
                EndDate = EndDate,
                Cycle = this.Cycle // Bind this via UI
            })
            .ToList();

    }

    public IActionResult OnPostSubmit()
    {

        // Serialize complex object to JSON
        //string serializedData = JsonSerializer.Serialize(EmployeeAttendanceData);
        //string startDateString = StartDate.ToString("yyyy-MM-dd");
        //string endDateString = EndDate.ToString("yyyy-MM-dd");
        //string cycleString = Cycle;

        string serializedData = JsonSerializer.Serialize(EmployeeAttendanceData);
        string startDateString = StartDate.ToString("yyyy-MM-dd");
        string endDateString = EndDate.ToString("yyyy-MM-dd");
        string cycleString = Cycle;


        // 📝 Assign to TempData
        TempData["EmployeeAttendanceData"] = serializedData;
        TempData["StartDate"] = startDateString;
        TempData["EndDate"] = endDateString;
        TempData["Cycle"] = cycleString;

        return RedirectToPage("Step3_Overtime");
    }


}
