using eLibrary.ViewModels;

namespace eLibrary.BusinessLayer.Services.Interfaces
{
    public interface IBookService
    {
        public Task<List<BookVM>> GetBooks();
    }
}
