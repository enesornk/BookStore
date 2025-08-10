using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BookStore.Shared.Models
{
    public class Category
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [StringLength(200)]
        public string? ImageUrl { get; set; }
        
        // Kitap sayısı için (veritabanında saklanmaz, sadece hesaplanır)
        [NotMapped]
        public int BookCount { get; set; }
        
        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
} 