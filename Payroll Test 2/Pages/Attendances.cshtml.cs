using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Data;
using Payroll_Test_2.Pages.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Payroll_Test_2.Pages
{
    public class AttendancesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AttendancesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Attendance> AttendanceRecords { get; set; }
        public DateTime SelectedDate { get; set; }
        public DateTime PreviousDate => SelectedDate.AddDays(-1);
        public DateTime NextDate => SelectedDate.AddDays(1);
        public string ChartDataJson { get; set; }
        public string AttendanceMonthJson { get; set; }


        public async Task<IActionResult> OnGetAsync(string date)
        {
            if (!DateTime.TryParse(date, out DateTime parsedDate))
            {
                parsedDate = DateTime.Today; // Default to today if invalid
            }

            SelectedDate = parsedDate;

            AttendanceRecords = await _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.Date.Date == SelectedDate.Date)
                .ToListAsync();

            // Convert data to JSON format for Chart.js
            var chartData = AttendanceRecords.Select(a => new
            {
                Employee = $"{a.Employee?.FirstName} {a.Employee?.LastName}",
                CheckInTime = a.CheckInTime?.Hour ?? 0,
                CheckOutTime = a.CheckOutTime?.Hour ?? 24,
                WorkHours = (a.CheckOutTime?.Hour ?? 24) - (a.CheckInTime?.Hour ?? 0)
            }).ToList();

            ChartDataJson = JsonSerializer.Serialize(chartData);
            return Page();
        }


    }
}
