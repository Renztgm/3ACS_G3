using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Pages.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Payroll_Test_2.Data; // Add this

namespace Payroll_Test_2.Pages
{

    [Authorize]
    public class AttendancesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AttendancesModel> _logger;

        public AttendancesModel(ApplicationDbContext context, ILogger<AttendancesModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Attendance> AttendanceRecords { get; set; }
        public DateTime SelectedDate { get; set; }
        public int EmployeeId { get; set; }
        public int AttendanceId { get; set; }
        public Employee Employee { get; set; }
        public DateTime Date { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public DateTime? LunchStartTime { get; set; } // ✅ Added
        public DateTime? LunchEndTime { get; set; } // ✅ Added
        public string ChartDataJson { get; set; }  // ✅ ADD THIS

        public DateTime PreviousDate => SelectedDate.AddDays(-1); // ✅ Added for Previous Day Button
        public DateTime NextDate => SelectedDate.AddDays(1); // ✅ Added for Next Day Button

        public async Task<IActionResult> OnGetAsync(int? id, string date)
        {
            if (!DateTime.TryParse(date, out DateTime parsedDate))
            {
                parsedDate = DateTime.Today;
            }

            SelectedDate = parsedDate;

            if (id == null)
            {
                _logger.LogWarning("Employee ID is null");
                return NotFound();
            }

            EmployeeId = id.Value;

            var attendanceQuery = _context.Attendance
                .Include(a => a.Employee)
                .Where(a => a.Date.HasValue && a.Date.Value.Date == SelectedDate.Date && a.Employee.EmployeeId == id);

            // ✅ Ensure correct date filtering
            AttendanceRecords = await _context.Attendance
                .Include(a => a.Employee)
                .Where(a => a.Date.HasValue && a.Date.Value.Date == SelectedDate.Date && a.Employee.EmployeeId == EmployeeId)
                .ToListAsync();

            _logger.LogInformation($"Fetched {AttendanceRecords.Count} records for Employee ID {EmployeeId} on {SelectedDate:yyyy-MM-dd}");

            // ✅ Format data correctly for the Chart.js graph
            ChartDataJson = JsonSerializer.Serialize(AttendanceRecords.Select(a => new
            {
                Employee = $"{a.Employee?.FirstName} {a.Employee?.LastName}",
                CheckInTime = a.CheckInTime?.Hour ?? 0,
                LunchStartTime = a.LunchStartTime?.Hour ?? 0,  // ✅ Added
                LunchEndTime = a.LunchEndTime?.Hour ?? 0,  // ✅ Added
                CheckOutTime = a.CheckOutTime?.Hour ?? 24,
                WorkHours = WorkHourss
            }).ToList());

            return Page();
        }

        public double WorkHourss
        {
            get
            {
                if (CheckInTime == null || CheckOutTime == null) return 0;
                return (CheckOutTime.Value - CheckInTime.Value).TotalHours;
            }
        }
    }
}
