using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Todo.Infrastructure.Identity;
using Todo.WebApi.ViewModels;

namespace Todo.WebApi.Controllers.Identity;

public class AccountController : Controller
{
    private readonly ILogger _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(
        ILogger<AccountController> logger,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (string.IsNullOrEmpty(returnUrl))
        {
            _logger.LogInformation("Logging in with no return url.");
        }
        else
        {
            _logger.LogInformation("Logging in with return url '{returnUrl}'.", returnUrl);
        }

        ViewData["ReturnUrl"] = returnUrl;

        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        ViewData["ReturnUrl"] = model.ReturnUrl;

        if (ModelState.IsValid)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(
                model.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (!signInResult.Succeeded)
            {
                return BadRequest("Sign in failed.");
            }

            var applicationUser = await _userManager.FindByEmailAsync(model.UserName);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, applicationUser.Id),
                new Claim(ClaimTypes.Email, applicationUser.Email),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            if (Url.IsLocalUrl(model.ReturnUrl))
            {
                _logger.LogInformation("Redirecting to local return url '{returnUrl}'.", model.ReturnUrl);

                return Redirect(model.ReturnUrl);
            }

            _logger.LogInformation("Redirecting to home.");

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        await HttpContext.SignOutAsync();

        return RedirectToAction(nameof(HomeController.Index), "Home");
    }
}
