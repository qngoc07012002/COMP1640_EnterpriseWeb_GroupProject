using GreenwichUniversityMagazine.Authentication;
using GreenwichUniversityMagazine.Serivces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace GreenwichUniversityMagazine.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthentication()]
    public class EmailController : Controller
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SendCode(string email)
        {
            Random random = new Random();
            string code = random.Next(100000, 999999).ToString();
            var subject = "Confirmation Code";
            var message = $"Your confirmation code is: {code}";
            await _emailService.SendEmailAsync(email, subject, message);
            return RedirectToAction("Index");
        }
    }
}
