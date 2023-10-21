using eLibrary.BusinessLayer.Services.Interfaces;
using eLibrary.DataLayers.Context;
using eLibrary.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Dapper;

namespace eLibrary.BusinessLayer.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly eLibraryDbContext _eLibraryDbContext;
        public BookService(eLibraryDbContext eLibraryDbContext) 
        {
            _eLibraryDbContext = eLibraryDbContext;
        }

        public async Task<List<BookVM>> GetBooks()
        {
            try
            {
                var books = new List<BookVM>();
                using (var conn = new SqlConnection(_eLibraryDbContext.Database.GetConnectionString()))
                {
                    books = conn.Query<BookVM>(@"select 
                                                b.Title as Title,
                                                b.Description as Description,
                                                b.IsAvailable as IsAvailable,
                                                a.FullName as Author,
                                                'Images/bookSpeakingVolumes.jpeg' as BookPicURL
                                                from Books b
                                                inner join Authors a on a.ID = b.AuthorID")
                        .ToList();
                }
                return books;
            }
            catch(Exception ex)
            {
                throw new Exception("Error in retrieving books");
            }
        }
    }
}
