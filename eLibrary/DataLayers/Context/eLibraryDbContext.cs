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
            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(projectPath)
                .AddJsonFile("appsettings.json")
                .Build();
            string connectionString = configuration.GetConnectionString("eLibrary");

            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
