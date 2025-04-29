namespace Payroll_Test_2.Pages.Models
{
    public class Bonus
    {
        public int Id { get; set; }
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }

}
