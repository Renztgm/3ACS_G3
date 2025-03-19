using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Data;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Payroll_Test_2.Pages
{
    public class AttendanceMonthModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AttendanceMonthModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string AttendanceMonthJson { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound(); // No EmployeeID in the URL
            }

            var attendanceData = await _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.Employee.EmployeeId == id) // Filter by EmployeeID
                .Select(a => new
                {
                    Id = a.AttendanceID,
                    Name = a.Employee.FirstName + " " + a.Employee.LastName,
                    Date = a.Date.ToString("yyyy-MM-dd"),
                    Present = a.CheckInTime != null,
                    Time = a.CheckInTime != null
                        ? a.CheckInTime.Value.ToString("HH:mm") + " - " + (a.CheckOutTime.HasValue ? a.CheckOutTime.Value.ToString("HH:mm") : "N/A")
                        : "-"
                })
                .ToListAsync();

            AttendanceMonthJson = JsonSerializer.Serialize(attendanceData);
            return Page();
        }





        public class AttendanceRecord
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Date { get; set; }
            public bool Present { get; set; }
            public string Time { get; set; }
        }
    }
}
