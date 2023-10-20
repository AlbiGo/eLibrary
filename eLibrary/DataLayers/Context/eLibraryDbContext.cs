using eLibrary.DataLayers.Entities;
using Microsoft.EntityFrameworkCore;

namespace eLibrary.DataLayers.Context
{
    public class eLibraryDbContext : DbContext
    {
        public eLibraryDbContext() : base() { }
        public eLibraryDbContext(DbContextOptions<DbContext> options) :base (options) { }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<BookClient> BookClients  { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=WINDOWS-4PGG12B; Initial Catalog=eLibrary;Integrated Security=True;TrustServerCertificate=True");
        }
    }
}
