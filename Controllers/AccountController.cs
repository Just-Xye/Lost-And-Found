using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using LostAndFound.Data;
using LostAndFound.Models;

namespace LostAndFound.Controllers
{
    public class AccountController : Controller
    {
        private readonly LostAndFoundContext _context;

        public AccountController(LostAndFoundContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("AdminDashboard", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var admin = _context.Admins
                .FirstOrDefault(a => a.Username == username);

            if (admin == null || !BCrypt.Net.BCrypt.Verify(password, admin.Password))
            {
                ViewBag.Error = "Invalid credentials";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, admin.Username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal);
            return RedirectToAction("AdminDashboard", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("AdminDashboard", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(AdminAccount model, string ConfirmPassword)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                ViewBag.Error = "All fields are required.";
                return View();
            }

            if (model.Password.Length < 8)
            {
                ViewBag.Error = "Password must be at least 8 characters.";
                return View();
            }

            if (model.Password != ConfirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            if (_context.Admins.Any(a => a.Username == model.Username))
            {
                ViewBag.Error = "Username already exists.";
                return View();
            }

            model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

            _context.Admins.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Account created! Please log in.";
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login");
        }
    }
}