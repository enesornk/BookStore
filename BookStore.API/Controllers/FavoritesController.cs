using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStore.API.Data;
using BookStore.Shared.Models;

namespace BookStore.API.Controllers
{
    public class FavoriteRequest
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly BookStoreDbContext _context;

        public FavoritesController(BookStoreDbContext context)
        {
            _context = context;
        }

        // GET: api/favorites/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Favorite>>> GetUserFavorites(int userId)
        {
            return await _context.Favorites
                .Include(f => f.Book)
                .ThenInclude(b => b.Category)
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        // POST: api/favorites
        [HttpPost]
        public async Task<ActionResult<Favorite>> PostFavorite([FromBody] FavoriteRequest request)
        {
            try
            {
                // Log the incoming data
                Console.WriteLine($"Received favorite request - UserId: {request.UserId}, BookId: {request.BookId}");
                
                // Check if already exists
                var existingFavorite = await _context.Favorites
                    .FirstOrDefaultAsync(f => f.UserId == request.UserId && f.BookId == request.BookId);

                if (existingFavorite != null)
                {
                    Console.WriteLine($"Favorite already exists - ID: {existingFavorite.Id}");
                    return BadRequest("Bu kitap zaten favorilerinizde bulunuyor.");
                }
                
                // Create new favorite
                var favorite = new Favorite
                {
                    UserId = request.UserId,
                    BookId = request.BookId,
                    CreatedDate = DateTime.Now
                };
                
                // Add to database
                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();
                
                Console.WriteLine($"Favorite added successfully with ID: {favorite.Id}");
                
                return Ok(favorite);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding favorite: {ex.Message}");
                return BadRequest($"Favori eklenirken hata: {ex.Message}");
            }
        }

        // DELETE: api/favorites/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFavorite(int id)
        {
            var favorite = await _context.Favorites.FindAsync(id);
            if (favorite == null)
            {
                return NotFound();
            }

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
} 