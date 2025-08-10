using Microsoft.AspNetCore.Mvc;
using BookStore.MVC.Services;
using BookStore.Shared.Models;
using System.Text.Json;

namespace BookStore.MVC.Controllers
{
    public class CartController : Controller
    {
        private readonly IApiService _apiService;

        public CartController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            var cartItems = GetCartItems();
            ViewBag.CartItems = cartItems;
            ViewBag.TotalAmount = cartItems.Sum(item => item.Price * item.Quantity);
            ViewBag.Title = "Sepetim";
            

            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int bookId, int quantity = 1)
        {
            try
            {
                Console.WriteLine($"AddToCart: BookId={bookId}, Quantity={quantity}");
                
                var book = await _apiService.GetBookAsync(bookId);
                if (book == null || book.Id == 0)
                {
                    Console.WriteLine($"AddToCart: Book not found for BookId={bookId}");
                    TempData["Error"] = "Kitap bulunamadı.";
                    return RedirectToAction("Index", "Books");
                }

                Console.WriteLine($"AddToCart: Book found - {book.Title}");

                var cartItems = GetCartItems();
                Console.WriteLine($"AddToCart: Current cart has {cartItems.Count} items");
                
                var existingItem = cartItems.FirstOrDefault(item => item.BookId == bookId);

                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                    Console.WriteLine($"AddToCart: Updated existing item quantity to {existingItem.Quantity}");
                }
                else
                {
                    var newItem = new CartItem 
                    { 
                        BookId = bookId, 
                        Title = book.Title,
                        Author = book.Author,
                        Price = book.Price,
                        ImageUrl = book.ImageUrl,
                        Quantity = quantity 
                    };
                    cartItems.Add(newItem);
                    Console.WriteLine($"AddToCart: Added new item - {newItem.Title}");
                }

                SaveCartItems(cartItems);
                Console.WriteLine($"AddToCart: Cart saved with {cartItems.Count} items");
                
                TempData["Success"] = $"{book.Title} sepete eklendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddToCart: Error - {ex.Message}");
                TempData["Error"] = "Ürün sepete eklenirken bir hata oluştu.";
                return RedirectToAction("Index", "Books");
            }
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int bookId)
        {
            try
            {
                var cartItems = GetCartItems();
                var itemToRemove = cartItems.FirstOrDefault(item => item.BookId == bookId);

                if (itemToRemove != null)
                {
                    cartItems.Remove(itemToRemove);
                    SaveCartItems(cartItems);
                    return Json(new { success = true, message = "Ürün sepetten çıkarıldı" });
                }
                
                return Json(new { success = false, message = "Ürün bulunamadı" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int bookId, int quantity)
        {
            try
            {
                var cartItems = GetCartItems();
                var item = cartItems.FirstOrDefault(item => item.BookId == bookId);

                if (item != null)
                {
                    if (quantity <= 0)
                    {
                        cartItems.Remove(item);
                    }
                    else
                    {
                        item.Quantity = quantity;
                    }
                    SaveCartItems(cartItems);
                    
                    return Json(new { success = true, message = "Miktar güncellendi" });
                }
                
                return Json(new { success = false, message = "Ürün bulunamadı" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
            }
        }

        public IActionResult Checkout()
        {
            var cartItems = GetCartItems();
            if (!cartItems.Any())
            {
                TempData["Error"] = "Sepetiniz boş.";
                return RedirectToAction("Index");
            }

            ViewBag.CartItems = cartItems;
            ViewBag.TotalAmount = cartItems.Sum(item => item.Price * item.Quantity);
            ViewBag.Title = "Siparişi Tamamla";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Sipariş vermek için giriş yapmalısınız.";
                return RedirectToAction("Login", "Auth");
            }

            var cartItems = GetCartItems();
            if (!cartItems.Any())
            {
                TempData["Error"] = "Sepetiniz boş.";
                return RedirectToAction("Index");
            }

            try
            {
                var orderRequest = new OrderRequest
                {
                    UserId = int.Parse(userId),
                    TotalAmount = cartItems.Sum(item => item.Price * item.Quantity),
                    Items = cartItems.Select(item => new OrderItemRequest
                    {
                        BookId = item.BookId,
                        Quantity = item.Quantity,
                        Price = item.Price
                    }).ToList()
                };

                await _apiService.CreateOrderAsync(orderRequest);
                ClearCart();
                TempData["Success"] = "Siparişiniz başarıyla oluşturuldu.";
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                TempData["Error"] = "Sipariş oluşturulurken bir hata oluştu.";
                return RedirectToAction("Checkout");
            }
        }

        private List<CartItem> GetCartItems()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
                return new List<CartItem>();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<List<CartItem>>(cartJson, options) ?? new List<CartItem>();
        }

        private void SaveCartItems(List<CartItem> cartItems)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var cartJson = JsonSerializer.Serialize(cartItems, options);
            HttpContext.Session.SetString("Cart", cartJson);
        }

        private void ClearCart()
        {
            HttpContext.Session.Remove("Cart");
        }

        [HttpGet]
        public IActionResult GetCartCount()
        {
            var cartItems = GetCartItems();
            return Json(new { count = cartItems.Sum(item => item.Quantity) });
        }
    }

    public class CartItem
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
    }
} 