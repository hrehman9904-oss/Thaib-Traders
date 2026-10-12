using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TahibTraders.Data;
using TahibTraders.ViewModels;

namespace TahibTraders.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DbSeeder.AdminRole)]
    public class PasswordController : Controller
    {
        private readonly UserManager<IdentityUser> _users;
        private readonly SignInManager<IdentityUser> _signIn;

        public PasswordController(UserManager<IdentityUser> users, SignInManager<IdentityUser> signIn)
        {
            _users = users;
            _signIn = signIn;
        }

        [HttpGet]
        public IActionResult Index() => View(new ChangePasswordViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _users.GetUserAsync(User);
            if (user == null) return Challenge();

            var result = await _users.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            // Keep the admin signed in after the password changes
            await _signIn.RefreshSignInAsync(user);
            TempData["Success"] = "Your password was changed.";
            return RedirectToAction(nameof(Index));
        }
    }
}