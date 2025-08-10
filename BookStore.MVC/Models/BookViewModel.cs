using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.MVC.Models
{
    public class BookViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Kitap başlığı zorunludur.")]
        [StringLength(200, ErrorMessage = "Kitap başlığı en fazla 200 karakter olabilir.")]
        [Display(Name = "Kitap Başlığı")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Yazar adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Yazar adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Yazar")]
        public string Author { get; set; } = string.Empty;
        
        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        [Display(Name = "Fiyat")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }
        
        [StringLength(200, ErrorMessage = "Resim URL'si en fazla 200 karakter olabilir.")]
        [Display(Name = "Resim URL")]
        public string? ImageUrl { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        // Dropdown için SelectList
        public SelectList? Categories { get; set; }
        
        // Mevcut kategori adı (düzenleme için)
        public string? CurrentCategoryName { get; set; }
        
        // Resim dosyası için (ViewModel'de tutulmaz, sadece form binding için)
        [Display(Name = "Kitap Resmi")]
        public IFormFile? BookImage { get; set; }
    }
}
