using System.Text;
using System.Text.Json;
using BookStore.Shared.Models;

namespace BookStore.MVC.Services
{
    public interface IApiService
    {
        // Books CRUD
        Task<List<Book>> GetBooksAsync();
        Task<Book> GetBookAsync(int id);
        Task<Book> CreateBookAsync(Book book);
        Task<Book> UpdateBookAsync(int id, Book book);
        Task DeleteBookAsync(int id);
        Task<List<Book>> GetBooksByCategoryAsync(int categoryId);
        
        // Categories CRUD
        Task<List<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryAsync(int id);
        Task<Category> CreateCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(int id, Category category);
        Task DeleteCategoryAsync(int id);
        
        // Orders
        Task<List<Order>> GetOrdersAsync();
        Task<Order> GetOrderAsync(int id);
        Task<Order> CreateOrderAsync(OrderRequest request);
        
        // Favorites
        Task<List<Favorite>> GetUserFavoritesAsync(int userId);
        Task<Favorite> AddToFavoritesAsync(Favorite favorite);
        Task DeleteFavoriteAsync(int id);
        
        // Users
        Task<List<User>> GetUsersAsync();
        Task<User> GetUserAsync(int id);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(int id, User user);
        Task DeleteUserAsync(int id);
        Task<User> LoginAsync(LoginRequest loginRequest);
        Task<User> RegisterAsync(User user);
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7281/api/");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<Book>> GetBooksAsync()
        {
            var response = await _httpClient.GetAsync("books");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Book>>(content, _jsonOptions) ?? new List<Book>();
        }

        public async Task<Book> GetBookAsync(int id)
        {
            var response = await _httpClient.GetAsync($"books/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Book>(content, _jsonOptions) ?? new Book();
        }

        public async Task<List<Book>> GetBooksByCategoryAsync(int categoryId)
        {
            var response = await _httpClient.GetAsync($"books/category/{categoryId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Book>>(content, _jsonOptions) ?? new List<Book>();
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetAsync("categories");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Category>>(content, _jsonOptions) ?? new List<Category>();
        }

        public async Task<Category> GetCategoryAsync(int id)
        {
            var response = await _httpClient.GetAsync($"categories/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Category>(content, _jsonOptions) ?? new Category();
        }

        // Books CRUD
        public async Task<Book> CreateBookAsync(Book book)
        {
            var json = JsonSerializer.Serialize(book, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("books", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Book>(responseContent, _jsonOptions) ?? new Book();
        }

        public async Task<Book> UpdateBookAsync(int id, Book book)
        {
            var json = JsonSerializer.Serialize(book, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"books/{id}", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Book>(responseContent, _jsonOptions) ?? new Book();
        }

        public async Task DeleteBookAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"books/{id}");
            response.EnsureSuccessStatusCode();
        }

        // Categories CRUD
        public async Task<Category> CreateCategoryAsync(Category category)
        {
            var json = JsonSerializer.Serialize(category, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("categories", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Category>(responseContent, _jsonOptions) ?? new Category();
        }

        public async Task<Category> UpdateCategoryAsync(int id, Category category)
        {
            var json = JsonSerializer.Serialize(category, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"categories/{id}", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Category>(responseContent, _jsonOptions) ?? new Category();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"categories/{id}");
            response.EnsureSuccessStatusCode();
        }

        // Users CRUD
        public async Task<List<User>> GetUsersAsync()
        {
            var response = await _httpClient.GetAsync("users");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<User>>(content, _jsonOptions) ?? new List<User>();
        }

        public async Task<User> GetUserAsync(int id)
        {
            var response = await _httpClient.GetAsync($"users/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User>(content, _jsonOptions) ?? new User();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            var json = JsonSerializer.Serialize(user, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("users", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User>(responseContent, _jsonOptions) ?? new User();
        }

        public async Task<User> UpdateUserAsync(int id, User user)
        {
            var json = JsonSerializer.Serialize(user, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"users/{id}", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            
            // API'den boş response gelirse orijinal user'ı döndür
            if (string.IsNullOrEmpty(responseContent))
            {
                return user;
            }
            
            try
            {
                return JsonSerializer.Deserialize<User>(responseContent, _jsonOptions) ?? user;
            }
            catch (JsonException)
            {
                // JSON parse hatası durumunda orijinal user'ı döndür
                return user;
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"users/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            var response = await _httpClient.GetAsync("orders");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Order>>(content, _jsonOptions) ?? new List<Order>();
        }

        public async Task<Order> GetOrderAsync(int id)
        {
            var response = await _httpClient.GetAsync($"orders/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Order>(content, _jsonOptions) ?? new Order();
        }

        public async Task<Order> CreateOrderAsync(OrderRequest request)
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("orders", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Order>(responseContent, _jsonOptions) ?? new Order();
        }

        public async Task<List<Favorite>> GetUserFavoritesAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"favorites/user/{userId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Favorite>>(content, _jsonOptions) ?? new List<Favorite>();
        }

        public async Task<Favorite> AddToFavoritesAsync(Favorite favorite)
        {
            try
            {
                Console.WriteLine($"ApiService: Serializing favorite - UserId: {favorite.UserId}, BookId: {favorite.BookId}");
                
                // Create request object
                var request = new { UserId = favorite.UserId, BookId = favorite.BookId };
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                Console.WriteLine($"ApiService: JSON: {json}");
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                Console.WriteLine($"ApiService: Sending POST request to favorites endpoint");
                
                var response = await _httpClient.PostAsync("favorites", content);
                Console.WriteLine($"ApiService: Response status: {response.StatusCode}");
                
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"ApiService: Response content: {responseContent}");
                
                response.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<Favorite>(responseContent, _jsonOptions) ?? new Favorite();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ApiService: Error in AddToFavoritesAsync: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteFavoriteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"favorites/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<User> LoginAsync(LoginRequest loginRequest)
        {
            var json = JsonSerializer.Serialize(loginRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("users/login", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User>(responseContent, _jsonOptions) ?? new User();
        }

        public async Task<User> RegisterAsync(User user)
        {
            var json = JsonSerializer.Serialize(user, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("users/register", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User>(responseContent, _jsonOptions) ?? new User();
        }
    }

    public class OrderRequest
    {
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
    }

    public class OrderItemRequest
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
} 