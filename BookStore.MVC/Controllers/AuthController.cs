using Microsoft.AspNetCore.Mvc;
using BookStore.MVC.Services;
using BookStore.Shared.Models;
using System.Text.Json;

namespace BookStore.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IApiService _apiService;

        public AuthController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Login()
        {
            ViewBag.Title = "Giriş Yap";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                var loginRequest = new BookStore.MVC.Services.LoginRequest { Email = username, Password = password };
                var user = await _apiService.LoginAsync(loginRequest);

                if (user != null)
                {
                    // Session'a kullanıcı bilgilerini kaydet
                    HttpContext.Session.SetString("User", JsonSerializer.Serialize(user));
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("UserRole", user.Role);

                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    TempData["Error"] = "Geçersiz kullanıcı adı veya şifre.";
                    return View();
                }
            }
            catch
            {
                TempData["Error"] = "Giriş yapılırken bir hata oluştu.";
                return View();
            }
        }

        public IActionResult Register()
        {
            ViewBag.Title = "Üye Ol";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string password)
        {
            try
            {
                var user = new User
                {
                    Username = username,
                    Email = email,
                    Password = password,
                    Role = "User"
                };

                var result = await _apiService.RegisterAsync(user);
                if (result != null)
                {
                    TempData["Success"] = "Üyeliğiniz başarıyla oluşturuldu. Giriş yapabilirsiniz.";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["Error"] = "Üyelik oluşturulurken bir hata oluştu.";
                    return View();
                }
            }
            catch
            {
                TempData["Error"] = "Üyelik oluşturulurken bir hata oluştu.";
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }


} 