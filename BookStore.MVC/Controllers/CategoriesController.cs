using Microsoft.AspNetCore.Mvc;
using BookStore.MVC.Services;
using BookStore.Shared.Models;

namespace BookStore.MVC.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IApiService _apiService;

        public CategoriesController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _apiService.GetCategoriesAsync();
                ViewBag.Categories = categories;
                ViewBag.Title = "Kategoriler";
                return View();
            }
            catch
            {
                ViewBag.Categories = new List<Category>();
                ViewBag.Title = "Kategoriler";
                return View();
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var category = await _apiService.GetCategoryAsync(id);
                var books = await _apiService.GetBooksByCategoryAsync(id);

                ViewBag.Category = category;
                ViewBag.Books = books;
                ViewBag.Title = category.Name;
                return View();
            }
            catch
            {
                ViewBag.Category = new Category();
                ViewBag.Books = new List<Book>();
                ViewBag.Title = "Kategori";
                return View();
            }
        }
    }
} 