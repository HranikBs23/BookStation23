using BookStation.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookStation.Data
{
    public class BookStationDbContext:DbContext
    {
        public BookStationDbContext(DbContextOptions options) : base(options) { 
        
        
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

    }
}
