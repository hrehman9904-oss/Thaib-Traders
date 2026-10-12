using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TahibTraders.Data;
using TahibTraders.ViewModels;

namespace TahibTraders.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signIn;
        private readonly UserManager<IdentityUser> _users;

        public AccountController(SignInManager<IdentityUser> signIn, UserManager<IdentityUser> users)
        {
            _signIn = signIn;
            _users = users;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);

            var user = await _users.FindByEmailAsync(model.Email);
            if (user != null && await _users.IsInRoleAsync(user, DbSeeder.AdminRole))
            {
                var result = await _signIn.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    if (Url.IsLocalUrl(returnUrl)) return LocalRedirect(returnUrl);
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Too many failed attempts. Please try again in a few minutes.");
                    return View(model);
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signIn.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}