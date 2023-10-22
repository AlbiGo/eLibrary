using eLibrary.ViewModels;

namespace eLibrary.BusinessLayer.Services.Interfaces
{
    public interface IBookService
    {
        public Task<List<BookVM>> GetBooks();
        public Task<BookVM> GetBookByID(int bookID);
        public Task<List<BookVM>> GetByAuthorName(string authorName);
    }
}
