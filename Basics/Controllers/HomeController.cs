using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Basics.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        public IActionResult Authenticate()
        {
            // Create claims and user
            var grandmasClaims = new List<Claim>
            {
                new(ClaimTypes.Name, "Bob"),
                new(ClaimTypes.Email, "bob@example.com"),
                new("Grandma.Says", "Very nice boy")
            };
            
            var licenseClaims = new List<Claim>
            {
                new(ClaimTypes.Name, "Bob K Foo"),
                new("DriverLicense", "A+")
            };

            var grandmaIdentity = new ClaimsIdentity(grandmasClaims, "Grandma Identity");
            var licenseIdentity = new ClaimsIdentity(licenseClaims, "Government");

            var userPrincipal = new ClaimsPrincipal(new[] {grandmaIdentity, licenseIdentity});

            // Sign in user (set cookie)
            HttpContext.SignInAsync(userPrincipal);
            
            // Redirect
            return RedirectToAction(nameof(Index));
        }
    }
}