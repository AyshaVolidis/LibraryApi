using LibraryApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Data
{
    public class AppDatabase:IdentityDbContext<AppUser>
    {
        public AppDatabase(DbContextOptions options):base(options)
        {
            
        }

        public DbSet<Book> tblBook { get; set; }

        public DbSet<Author> tblAuthor { get; set; }
    }
}
