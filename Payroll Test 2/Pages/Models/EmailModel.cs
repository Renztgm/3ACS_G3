using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Payroll_Test_2.Services;
using System.Threading.Tasks;

namespace Payroll_Test_2.Pages
{
    public class EmailModel : PageModel
    {
        private readonly IEmailService _emailService;

        [BindProperty]
        public string EmailAddress { get; set; }

        [BindProperty]
        public string EmailContent { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public EmailModel(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public void OnGet()
        {
            //// Default values can be set here if needed
            //EmailAddress = "baconjhonlorenz@gmail.com";
            //EmailContent = "The 6 months already passed.";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(EmailAddress) || string.IsNullOrEmpty(EmailContent))
            {
                StatusMessage = "Email address and content are required.";
                return Page();
            }

            bool result = await _emailService.SendPayrollNotificationAsync(EmailAddress, EmailContent);

            if (result)
            {
                StatusMessage = "Email sent successfully!";
            }
            else
            {
                StatusMessage = "Failed to send email. Please check logs for details.";
            }

            return Page();
        }
    }
}