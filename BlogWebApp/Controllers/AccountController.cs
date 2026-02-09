using BlogWebApp.Services.UserServices;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> Register(
            RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterAsync(
                    registerViewModel.UserName,
                    registerViewModel.Email,
                    registerViewModel.Password);

                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");

                foreach (var error in result.Errors)
                {
                    string errorMessage = error.Code switch
                    {
                        "DuplicateUserName" => "Имя пользователя уже занято.",
                        "DuplicateEmail" => "Email уже используется.",
                        "PasswordTooShort" =>
                            "Пароль должен быть не менее 8 символов.",

                        "PasswordRequiresNonAlphanumeric" =>
                            "Пароль должен содержать хотя бы один спецсимвол.",

                        "PasswordRequiresLower" =>
                            "Пароль должен содержать хотя бы одну строчную букву (a–z).",

                        "PasswordRequiresUpper" =>
                            "Пароль должен содержать хотя бы одну заглавную букву (A–Z).",

                        "PasswordRequiresDigit" =>
                            "Пароль должен содержать хотя бы одну цифру (0–9).",
                        _ => error.Description
                    };
                    ModelState.AddModelError(string.Empty, errorMessage);
                }
            }

            return View(registerViewModel);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe);

                if (result)
                    return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Неверный логин или пароль.");
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
