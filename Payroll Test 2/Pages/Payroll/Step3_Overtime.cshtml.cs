using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Payroll_Test_2.Pages.Helpers;
using Payroll_Test_2.Pages.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using static Step2_CalculateWorkedHoursModel;

namespace Payroll_Test_2.Pages.Payroll
{
    public class Step3_OvertimeModel : PageModel
    {
        public List<Step2_CalculateWorkedHoursModel.EmployeeAttendanceSummary> EmployeeAttendanceData { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Cycle { get; set; }

        public void OnGet()
        {
            // ✅ Grab values from TempData into variables
            string jsonData = TempData["EmployeeAttendanceData"] as string;
            string startDateString = TempData["StartDate"] as string;
            string endDateString = TempData["EndDate"] as string;
            string cycleString = TempData["Cycle"] as string;

            Debug.WriteLine($"{jsonData} {startDateString} {endDateString} {cycleString}");
        }
    }
}
