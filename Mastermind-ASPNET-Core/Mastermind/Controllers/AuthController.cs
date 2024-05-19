using Mastermind.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Mastermind.DataAccessLayer;
using Mastermind.Models;
using Mastermind.Resources;
using System.Security.Claims;
using Mastermind.Helpers;

namespace Mastermind.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            AuthLoginVM viewModel = new();

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Login(AuthLoginVM viewModel, string? returnurl)
        {
            if (ModelState.IsValid)
            {
                Member? user = new DAL().MemberFact.GetByUsername(viewModel.Username);

                if (user != null)
                {
                    //bool valid = viewModel.Password == user.Password;
                    bool valid = CryptographyHelper.ValidateHashedPassword(viewModel.Password, user.Password);

                    if (valid)
                    {
                        var identity = new ClaimsIdentity(new[] {
                                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                                new Claim(ClaimTypes.Name, user.Username),
                                new Claim(ClaimTypes.Email, user.Email),
                                new Claim(ClaimTypes.Role, user.Role)
                            }, CookieAuthenticationDefaults.AuthenticationScheme);

                        HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                        if (!string.IsNullOrWhiteSpace(returnurl) && Url.IsLocalUrl(returnurl))
                        {
                            if (user.Role == Member.ROLE_STANDARD && returnurl.ToLower().StartsWith("/admin"))
                            {
                                return RedirectToAction("Index", "Home", new { Area = "" });
                            }

                            return LocalRedirect(returnurl);
                        }
                        else if (user.Role == Member.ROLE_ADMIN)
                        {
                            return RedirectToAction("Index", "Home", new { Area = "Admin" });
                        }

                        return RedirectToAction("Index", "Home", new { Area = "" });
                    }
                }

                ModelState.AddModelError("Password", Resource.InvalidUsernamePassword);
            }

            return View(viewModel);
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
    }
}
