using Microsoft.AspNetCore.Mvc;
using BookStore.MVC.Services;
using BookStore.Shared.Models;

namespace BookStore.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApiService _apiService;

        public HomeController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var books = await _apiService.GetBooksAsync();
                var categories = await _apiService.GetCategoriesAsync();

                ViewBag.FeaturedBooks = books.Take(6).ToList();
                ViewBag.Categories = categories;

                return View();
            }
            catch
            {
                ViewBag.FeaturedBooks = new List<Book>();
                ViewBag.Categories = new List<Category>();
                return View();
            }
        }

        public IActionResult About()
        {
            ViewBag.Title = "Hakkımızda";
            ViewBag.Message = "Kitap Mağazamız 2025 yılında kurulmuş olup, en kaliteli kitapları en uygun fiyatlarla sunmaktadır.";
            
            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Title = "Bize Ulaşın";
            ViewBag.Email = "enesor.61.54@gmail.com";
            ViewBag.Phone = "+90 212 555 0123";
            ViewBag.Address = "Sakarya, Türkiye";
            
            return View();
        }
    }
}
