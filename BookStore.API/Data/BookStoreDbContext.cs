using Microsoft.EntityFrameworkCore;
using BookStore.Shared.Models;

namespace BookStore.API.Data
{
    public class BookStoreDbContext : DbContext
    {
        public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2024, 1, 1);
            
            // Admin user
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@bookstore.com",
                Password = "admin123", // In real app, this should be hashed
                Role = "Admin",
                CreatedDate = seedDate
            });

            // Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Roman", Description = "Roman türü kitaplar" },
                new Category { Id = 2, Name = "Bilim Kurgu", Description = "Bilim kurgu türü kitaplar" },
                new Category { Id = 3, Name = "Tarih", Description = "Tarih türü kitaplar" },
                new Category { Id = 4, Name = "Bilim", Description = "Bilim türü kitaplar" }
            );

            // Books
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    Title = "Suç ve Ceza",
                    Author = "Fyodor Dostoyevski",
                    Description = "Klasik Rus edebiyatının başyapıtlarından",
                    Price = 45.00m,
                    CategoryId = 1,
                    CreatedDate = seedDate
                },
                new Book
                {
                    Id = 2,
                    Title = "1984",
                    Author = "George Orwell",
                    Description = "Distopik roman",
                    Price = 35.00m,
                    CategoryId = 2,
                    CreatedDate = seedDate
                },
                new Book
                {
                    Id = 3,
                    Title = "Osmanlı Tarihi",
                    Author = "Halil İnalcık",
                    Description = "Osmanlı İmparatorluğu tarihi",
                    Price = 60.00m,
                    CategoryId = 3,
                    CreatedDate = seedDate
                }
            );
        }
    }
} 