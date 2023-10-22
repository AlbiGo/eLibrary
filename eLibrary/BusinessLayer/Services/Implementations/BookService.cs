using eLibrary.BusinessLayer.Services.Interfaces;
using eLibrary.DataLayers.Context;
using eLibrary.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Dapper;
using eLibrary.DataLayers.Entities;

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
                                                b.ID,
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

        public async Task<BookVM> GetBookByID(int bookID)
        {
            try
            {
                //Menyra 1
                var book = _eLibraryDbContext.Books
                    .Include(p => p.Author)
                    .Include(p => p.Kategoria)
                    .Where(p => p.ID == bookID)
                    .FirstOrDefault();


                if (book == null)
                {
                    throw new Exception("Book does not exist");
                }

                //Mapping 
                var bookVM = new BookVM()
                {
                    Author = book.Author.FullName,
                    IsAvailable = book.IsAvailable,
                    BookPicURL = "/Images/bookSpeakingVolumes.jpeg",
                    Description = book.Description,
                    ID = book.ID,
                    Title = book.Title,
                    Kategoria = book.Kategoria
                };

                return bookVM;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in getting book");
            }
        }


        public async Task<BookVM> GetBookByID_V2(int bookID)
        {
            try
            {
                var bookVM = new BookVM();
                using (var conn = new SqlConnection(_eLibraryDbContext.Database.GetConnectionString()))
                {
                    bookVM = conn.Query<BookVM>(@"select 
                                                b.ID,
                                                b.Title as Title,
                                                b.Description as Description,
                                                b.IsAvailable as IsAvailable,
                                                a.FullName as Author,
                                                'Images/bookSpeakingVolumes.jpeg' as BookPicURL,
                                                Kategoria
                                                from Books b
                                                inner join Authors a on a.ID = b.AuthorID
                                                where @bookID = b.ID and
                                                      @created < getdate()",
                                                new 
                                                {
                                                    bookID = bookID
                                                })
                        .FirstOrDefault();
                }
                return bookVM;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in getting book");
            }
        }

        public async Task<List<BookVM>> GetByAuthorName(string authorName)
        {
            try
            {
                var books = _eLibraryDbContext.Books
                    .Include(p => p.Author)
                    .Where(p => p.Author.FullName.Contains(authorName))
                    .ToList();

                var booksVM = new List<BookVM>();

                foreach (var book in books)
                {
                    booksVM.Add(new BookVM()
                    { 
                        Author = book.Author.FullName,
                        IsAvailable = true,
                        BookPicURL = "/Images/bookSpeakingVolumes.jpeg",
                        Description = book.Description,
                        ID = book.ID,
                        Kategoria= book.Kategoria,
                        Title = book.Title
                    });
                }
                return booksVM;
            }
            catch(Exception ex)
            {
                throw new Exception("Error in getting books");
            }
        }

        public async Task<BookVM> GetBookByID_V3(int bookID)
        {
            try
            {
                var bookVM = (from book in _eLibraryDbContext.Books
                              .Include(p => p.Author)
                              where book.ID == bookID
                              select new BookVM()
                              {
                                  Author = book.Author.FullName,
                                  IsAvailable = book.IsAvailable,
                                  BookPicURL = "Images/bookSpeakingVolumes.jpeg",
                                  Description = book.Description,
                                  ID = book.ID,
                                  Title = book.Title
                              }).FirstOrDefault();

                return bookVM;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in getting book");
            }
        }
    }
}
