using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Payroll_Test_2.Data;

namespace Payroll_Test_2.Controllers

{
    public class AttendancesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // View attendance records for a specific date
        public async Task<IActionResult> Index(DateTime? date)
        {
            if (date == null)
            {
                date = DateTime.Today; // Default to today if no date is provided
            }

            var attendanceRecords = await _context.Attendance
                .Include(a => a.Employee)
                .Where(a => a.Date == date)
                .ToListAsync();

            ViewBag.SelectedDate = date.Value.ToString("yyyy-MM-dd"); // Pass the selected date to the view

            return View(attendanceRecords);
        }
    }
}
