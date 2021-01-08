using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;

        public HomeController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Index() => View();

        [Authorize]
        public IActionResult Secret() => View();

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Get the user
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return Unauthorized();

            // Sign in user
            var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);

            if (signInResult.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            return Unauthorized();
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            // Create user
            var user = new IdentityUser
            {
                UserName = username
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded) return BadRequest();

            // Generation of email
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(
                nameof(VerifyEmail),
                "Home",
                new {userId = user.Id, code},
                Request.Scheme,
                Request.Host.ToString());

            // Send test email -- make sure FakeSMTP is open
            await _emailService.SendAsync(
                "test@example.com",
                "Email Verified",
                $"<a href='{link}'>Verify Email</a>",
                true);

            // Redirect to Email verification page
            return RedirectToAction(nameof(EmailVerification));
        }

        public IActionResult EmailVerification() => View();

        public async Task<IActionResult> VerifyEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.ConfirmEmailAsync(user, code);
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}