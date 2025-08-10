using Microsoft.AspNetCore.Mvc;
using BookStore.MVC.Services;
using BookStore.Shared.Models;

namespace BookStore.MVC.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly IApiService _apiService;

        public FavoritesController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Favorilerinizi görmek için giriş yapmalısınız.";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var favorites = await _apiService.GetUserFavoritesAsync(int.Parse(userId));
                ViewBag.Favorites = favorites;
                ViewBag.Title = "Favorilerim";
                return View();
            }
            catch
            {
                ViewBag.Favorites = new List<Favorite>();
                ViewBag.Title = "Favorilerim";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int bookId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Favoriye eklemek için giriş yapmalısınız.";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                Console.WriteLine($"MVC: Adding favorite - UserId: {userId}, BookId: {bookId}");
                
                var favorite = new Favorite
                {
                    UserId = int.Parse(userId),
                    BookId = bookId
                };

                Console.WriteLine($"MVC: Created favorite object - UserId: {favorite.UserId}, BookId: {favorite.BookId}");

                var result = await _apiService.AddToFavoritesAsync(favorite);
                Console.WriteLine($"MVC: API call successful, result ID: {result?.Id}");
                
                TempData["Success"] = "Kitap favorilere eklendi.";
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("400"))
            {
                // API'den gelen 400 Bad Request'i handle et
                Console.WriteLine($"MVC: Duplicate favorite detected: {ex.Message}");
                TempData["Error"] = "Bu kitap zaten favorilerinizde bulunuyor.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MVC: Error adding favorite: {ex.Message}");
                Console.WriteLine($"MVC: Inner exception: {ex.InnerException?.Message}");
                TempData["Error"] = $"Favoriye eklenirken bir hata oluştu: {ex.Message}";
            }

            return RedirectToAction("Details", "Books", new { id = bookId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(int favoriteId)
        {
            try
            {
                await _apiService.DeleteFavoriteAsync(favoriteId);
                TempData["Success"] = "Kitap favorilerden çıkarıldı.";
            }
            catch
            {
                TempData["Error"] = "Favoriden çıkarılırken bir hata oluştu.";
            }

            return RedirectToAction("Index");
        }
    }
} 