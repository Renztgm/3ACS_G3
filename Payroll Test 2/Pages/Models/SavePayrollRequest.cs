namespace Payroll_Test_2.Pages.Models
{
    public class SavePayrollRequest
    {
        public int EmployeeID { get; set; }
        public int? AttendanceID { get; set; }
        public int? LoanID { get; set; }
        public int? DeductionID { get; set; }
        public string PayrollCycle { get; set; }
        public DateTime PayrollStartDate { get; set; }
        public DateTime PayrollEndDate { get; set; }
        public decimal? TotalWorkedHours { get; set; }
        public decimal? OvertimeHours { get; set; }
        public decimal? Incentive { get; set; }
        public decimal? Bonus { get; set; }
        public int? LeaveID { get; set; }
    }

}
