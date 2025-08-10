using Microsoft.AspNetCore.Mvc;
using BookStore.MVC.Services;
using BookStore.Shared.Models;

namespace BookStore.MVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly IApiService _apiService;

        public BooksController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var books = await _apiService.GetBooksAsync();
                ViewBag.Books = books;
                ViewBag.Title = "Tüm Kitaplar";
                return View();
            }
            catch
            {
                ViewBag.Books = new List<Book>();
                ViewBag.Title = "Tüm Kitaplar";
                return View();
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var book = await _apiService.GetBookAsync(id);
                if (book == null || book.Id == 0)
                {
                    return NotFound();
                }

                ViewBag.Book = book;
                ViewBag.Title = book.Title;
                return View();
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Category(int id)
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