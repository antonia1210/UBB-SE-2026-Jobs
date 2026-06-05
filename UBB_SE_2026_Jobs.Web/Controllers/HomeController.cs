using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Web.Infrastructure;
using UBB_SE_2026_Jobs.Web.Models;
using System.Diagnostics;

namespace UBB_SE_2026_Jobs.Web.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAuthenticatedHome();
            }

            var selected = HttpContext.Session.GetString(SessionKeys.Mode);
            if (string.IsNullOrWhiteSpace(selected))
            {
                return View();
            }

            return RedirectToModeHome(selected);
        }

        [AllowAnonymous]
        public IActionResult SwitchMode()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAuthenticatedHome();
            }

            HttpContext.Session.Remove(SessionKeys.Mode);
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectMode(string mode)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAuthenticatedHome();
            }

            if (!AppModes.IsValid(mode))
            {
                TempData["ModeError"] = "Select a valid mode to continue.";
                return RedirectToAction(nameof(Index));
            }

            HttpContext.Session.SetString(SessionKeys.Mode, mode);
            return RedirectToModeHome(mode);
        }

        private IActionResult RedirectToModeHome(string mode)
        {
            if (string.Equals(mode, AppModes.User, StringComparison.OrdinalIgnoreCase))
            {
                var userLanding = Url.Action("Index", "UserProfile") ?? "/";
                return User.Identity?.IsAuthenticated == true
                    ? Redirect(userLanding)
                    : RedirectToAction("Login", "Account", new { returnUrl = userLanding });
            }

            if (string.Equals(mode, AppModes.Company, StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Matches");
            }

            TempData["ModeError"] = "Select a valid mode to continue.";
            return RedirectToAction(nameof(Index));
        }

        private IActionResult RedirectToAuthenticatedHome()
        {
            if (User.IsInRole("Recruiter"))
            {
                HttpContext.Session.SetString(SessionKeys.Mode, AppModes.Company);
                return RedirectToAction("Index", "Matches");
            }

            HttpContext.Session.SetString(SessionKeys.Mode, AppModes.User);
            return RedirectToAction("Index", "UserProfile");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
