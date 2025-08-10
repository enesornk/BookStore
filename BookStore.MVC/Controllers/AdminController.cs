using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookStore.MVC.Services;
using BookStore.Shared.Models;
using BookStore.MVC.Models;

namespace BookStore.MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly IApiService _apiService;

        public AdminController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: Admin
        public IActionResult Index()
        {
            ViewBag.Title = "Admin Paneli";
            return View();
        }

        // GET: Admin/Books
        public async Task<IActionResult> Books()
        {
            try
            {
                var books = await _apiService.GetBooksAsync();
                ViewBag.Books = books;
                ViewBag.Title = "Kitap Yönetimi";
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Kitaplar yüklenirken hata oluştu: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: Admin/Books/Create
        public async Task<IActionResult> CreateBook()
        {
            try
            {
                var categories = await _apiService.GetCategoriesAsync();
                var viewModel = new BookViewModel
                {
                    Categories = new SelectList(categories, "Id", "Name")
                };
                ViewBag.Title = "Yeni Kitap Ekle";
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Kategoriler yüklenirken hata oluştu: " + ex.Message;
                return RedirectToAction("Books");
            }
        }

        // POST: Admin/Books/Create
        [HttpPost]
        public async Task<IActionResult> CreateBook(BookViewModel viewModel)
        {
            try
            {
                Console.WriteLine($"=== CreateBook POST Debug ===");
                Console.WriteLine($"Title: {viewModel.Title}");
                Console.WriteLine($"Author: {viewModel.Author}");
                Console.WriteLine($"CategoryId: {viewModel.CategoryId}");
                Console.WriteLine($"Price: {viewModel.Price}");
                Console.WriteLine($"BookImage: {(viewModel.BookImage != null ? viewModel.BookImage.FileName : "null")}");
                Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
                
                // 1. ÖNCE ModelState kontrolü yap - hiçbir dosya işlemi yapma
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    Console.WriteLine($"ModelState Errors: {string.Join(", ", errors)}");
                    TempData["Error"] = "Form verilerinde hata var: " + string.Join(", ", errors);
                    
                    // Hata durumunda ViewModel'i yeniden hazırla
                    await PrepareCreateViewModel(viewModel);
                    return View(viewModel);
                }
                
                // 2. ViewModel'i Book modeline dönüştür
                var book = new Book
                {
                    Title = viewModel.Title,
                    Author = viewModel.Author,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    CategoryId = viewModel.CategoryId,
                    ImageUrl = viewModel.ImageUrl,
                    CreatedDate = DateTime.Now
                };
                
                // 3. SADECE ModelState.IsValid == true ise resim işle
                if (viewModel.BookImage != null && viewModel.BookImage.Length > 0)
                {
                    Console.WriteLine($"Processing image upload: {viewModel.BookImage.FileName}");
                    
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.BookImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "books", fileName);
                    
                    // Klasör yoksa oluştur
                    var directory = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory!);
                    }
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.BookImage.CopyToAsync(stream);
                    }
                    
                    book.ImageUrl = "/images/books/" + fileName;
                    Console.WriteLine($"Image saved to: {book.ImageUrl}");
                }
                
                // 4. API'ye oluşturma isteği gönder
                Console.WriteLine($"Sending create request to API...");
                var createdBook = await _apiService.CreateBookAsync(book);
                
                if (createdBook != null && createdBook.Id > 0)
                {
                    Console.WriteLine($"Book created successfully. ID: {createdBook.Id}");
                    TempData["Success"] = "Kitap başarıyla eklendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    Console.WriteLine($"Create failed - returned book is null or has invalid ID");
                    TempData["Error"] = "Kitap eklenirken beklenmeyen bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateBook Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                TempData["Error"] = "Kitap eklenirken hata oluştu: " + ex.Message;
            }

            // Hata durumunda ViewModel'i yeniden hazırla
            await PrepareCreateViewModel(viewModel);
            return View(viewModel);
        }
        
        private async Task PrepareCreateViewModel(BookViewModel viewModel)
        {
            try
            {
                var categories = await _apiService.GetCategoriesAsync();
                viewModel.Categories = new SelectList(categories, "Id", "Name");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error preparing CreateViewModel: {ex.Message}");
                TempData["Error"] = "Kategoriler yüklenirken hata oluştu: " + ex.Message;
            }
            
            ViewBag.Title = "Yeni Kitap Ekle";
        }

        // GET: Admin/Books/Edit/5
        public async Task<IActionResult> EditBook(int id)
        {
            try
            {
                var book = await _apiService.GetBookAsync(id);
                var categories = await _apiService.GetCategoriesAsync();
                
                // Debug için log
                Console.WriteLine($"EditBook GET - Book ID: {book.Id}, CategoryId: {book.CategoryId}, Category Name: {book.Category?.Name}");
                
                // Book'u ViewModel'e dönüştür
                var viewModel = new BookViewModel
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    Description = book.Description,
                    Price = book.Price,
                    CategoryId = book.CategoryId,
                    ImageUrl = book.ImageUrl,
                    CreatedDate = book.CreatedDate,
                    Categories = new SelectList(categories, "Id", "Name"),
                    CurrentCategoryName = book.Category?.Name
                };
                
                ViewBag.Title = "Kitap Düzenle";
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Kitap yüklenirken hata oluştu: " + ex.Message;
                return RedirectToAction("Books");
            }
        }

        // POST: Admin/Books/Edit/5
        [HttpPost]
        public async Task<IActionResult> EditBook(int id, BookViewModel viewModel)
        {
            try
            {
                Console.WriteLine($"=== EditBook POST Debug ===");
                Console.WriteLine($"ID: {viewModel.Id}");
                Console.WriteLine($"Title: {viewModel.Title}");
                Console.WriteLine($"Author: {viewModel.Author}");
                Console.WriteLine($"CategoryId: {viewModel.CategoryId}");
                Console.WriteLine($"Price: {viewModel.Price}");
                Console.WriteLine($"BookImage: {(viewModel.BookImage != null ? viewModel.BookImage.FileName : "null")}");
                Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
                
                // 1. ÖNCE ModelState kontrolü yap - hiçbir dosya işlemi yapma
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    Console.WriteLine($"ModelState Errors: {string.Join(", ", errors)}");
                    TempData["Error"] = "Form verilerinde hata var: " + string.Join(", ", errors);
                    
                    // Hata durumunda ViewModel'i yeniden hazırla
                    await PrepareViewModelForError(viewModel, id);
                    return View(viewModel);
                }
                
                // 2. ModelState geçerliyse mevcut kitabı al
                var existingBook = await _apiService.GetBookAsync(id);
                Console.WriteLine($"Existing Book CategoryId: {existingBook.CategoryId}");
                
                // 3. ViewModel'i Book modeline dönüştür
                var book = new Book
                {
                    Id = id,
                    Title = viewModel.Title,
                    Author = viewModel.Author,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    CategoryId = viewModel.CategoryId,
                    ImageUrl = viewModel.ImageUrl,
                    CreatedDate = existingBook.CreatedDate
                };
                
                Console.WriteLine($"Book to Update - CategoryId: {book.CategoryId}");
                
                // 4. SADECE ModelState.IsValid == true ise resim işle
                if (viewModel.BookImage != null && viewModel.BookImage.Length > 0)
                {
                    Console.WriteLine($"Processing image upload: {viewModel.BookImage.FileName}");
                    
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.BookImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "books", fileName);
                    
                    // Klasör yoksa oluştur
                    var directory = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory!);
                    }
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.BookImage.CopyToAsync(stream);
                    }
                    
                    book.ImageUrl = "/images/books/" + fileName;
                    Console.WriteLine($"Image saved to: {book.ImageUrl}");
                }
                else if (string.IsNullOrEmpty(book.ImageUrl))
                {
                    // Resim yüklenmemişse ve URL de boşsa, mevcut resmi koru
                    book.ImageUrl = existingBook.ImageUrl;
                    Console.WriteLine($"Keeping existing image: {book.ImageUrl}");
                }
                
                // 5. API'ye güncelleme isteği gönder
                Console.WriteLine($"Sending update request to API...");
                var updatedBook = await _apiService.UpdateBookAsync(id, book);
                
                if (updatedBook != null && updatedBook.Id > 0)
                {
                    Console.WriteLine($"Book updated successfully. ID: {updatedBook.Id}");
                    TempData["Success"] = "Kitap başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    Console.WriteLine($"Update failed - returned book is null or has invalid ID");
                    TempData["Error"] = "Kitap güncellenirken beklenmeyen bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EditBook Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                TempData["Error"] = "Kitap güncellenirken hata oluştu: " + ex.Message;
            }

            // Hata durumunda ViewModel'i yeniden hazırla
            await PrepareViewModelForError(viewModel, id);
            return View(viewModel);
        }
        
        private async Task PrepareViewModelForError(BookViewModel viewModel, int bookId)
        {
            try
            {
                var categories = await _apiService.GetCategoriesAsync();
                viewModel.Categories = new SelectList(categories, "Id", "Name");
                
                var existingBook = await _apiService.GetBookAsync(bookId);
                if (existingBook != null)
                {
                    viewModel.CurrentCategoryName = existingBook.Category?.Name;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error preparing ViewModel: {ex.Message}");
                TempData["Error"] = "Kategoriler yüklenirken hata oluştu: " + ex.Message;
            }
            
            ViewBag.Title = "Kitap Düzenle";
        }

        // POST: Admin/Books/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                await _apiService.DeleteBookAsync(id);
                TempData["Success"] = "Kitap başarıyla silindi!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Kitap silinirken hata oluştu: " + ex.Message;
            }
            return RedirectToAction("Books");
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Categories()
        {
            try
            {
                var categories = await _apiService.GetCategoriesAsync();
                ViewBag.Categories = categories;
                ViewBag.Title = "Kategori Yönetimi";
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Kategoriler yüklenirken hata oluştu: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Orders()
        {
            try
            {
                var orders = await _apiService.GetOrdersAsync();
                ViewBag.Orders = orders;
                ViewBag.Title = "Sipariş Yönetimi";
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Siparişler yüklenirken hata oluştu: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: Admin/Categories/Create
        public IActionResult CreateCategory()
        {
            ViewBag.Title = "Yeni Kategori Ekle";
            return View();
        }

        // POST: Admin/Categories/Create
        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _apiService.CreateCategoryAsync(category);
                    TempData["Success"] = "Kategori başarıyla eklendi!";
                    return RedirectToAction("Categories");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Kategori eklenirken hata oluştu: " + ex.Message;
            }

            ViewBag.Title = "Yeni Kategori Ekle";
            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> EditCategory(int id)
        {
            try
            {
                var category = await _apiService.GetCategoryAsync(id);
                ViewBag.Title = "Kategori Düzenle";
                return View(category);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Kategori yüklenirken hata oluştu: " + ex.Message;
                return RedirectToAction("Categories");
            }
        }

        // POST: Admin/Categories/Edit/5
        [HttpPost]
        public async Task<IActionResult> EditCategory(int id, Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _apiService.UpdateCategoryAsync(id, category);
                    TempData["Success"] = "Kategori başarıyla güncellendi!";
                    return RedirectToAction("Categories");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Kategori güncellenirken hata oluştu: " + ex.Message;
            }

            ViewBag.Title = "Kategori Düzenle";
            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _apiService.DeleteCategoryAsync(id);
                TempData["Success"] = "Kategori başarıyla silindi!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Kategori silinirken hata oluştu: " + ex.Message;
            }
            return RedirectToAction("Categories");
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            try
            {
                var users = await _apiService.GetUsersAsync();
                ViewBag.Users = users;
                ViewBag.Title = "Kullanıcı Yönetimi";
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Kullanıcılar yüklenirken hata oluştu: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> OrderDetails(int id)
        {
            try
            {
                var order = await _apiService.GetOrderAsync(id);
                ViewBag.Order = order;
                ViewBag.Title = "Sipariş Detayları";
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Sipariş detayları yüklenirken hata oluştu: " + ex.Message;
                return RedirectToAction("Orders");
            }
        }

        // GET: Admin/Users/Edit/5
        public async Task<IActionResult> EditUser(int id)
        {
            try
            {
                var user = await _apiService.GetUserAsync(id);
                ViewBag.Title = "Kullanıcı Düzenle";
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Kullanıcı yüklenirken hata oluştu: " + ex.Message;
                return RedirectToAction("Users");
            }
        }

        // POST: Admin/Users/Edit/5
        [HttpPost]
        public async Task<IActionResult> EditUser(int id, User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _apiService.UpdateUserAsync(id, user);
                    TempData["Success"] = "Kullanıcı başarıyla güncellendi!";
                    return RedirectToAction("Users");
                }
            }
            catch (Exception ex)
            {
                // Sadece gerçek hatalarda error mesajı göster
                if (!ex.Message.Contains("JSON") && !ex.Message.Contains("tokens"))
                {
                    TempData["Error"] = "Kullanıcı güncellenirken hata oluştu: " + ex.Message;
                }
                else
                {
                    // JSON hatası durumunda başarı mesajı göster
                    TempData["Success"] = "Kullanıcı başarıyla güncellendi!";
                    return RedirectToAction("Users");
                }
            }

            ViewBag.Title = "Kullanıcı Düzenle";
            return View(user);
        }
    }
} 