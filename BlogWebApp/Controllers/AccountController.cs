using BlogWebApp.Models;
using BlogWebApp.Services.UserServices;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebApp.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Username, Email = model.Email, PasswordHash = model.Password };
                var result = await _userService.RegisterAsync(user.UserName, user.Email, user.PasswordHash);

                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginAsync(model.Email, model.Password, model.RememberMe);

                if (result)
                    return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Неверный логин или пароль.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
