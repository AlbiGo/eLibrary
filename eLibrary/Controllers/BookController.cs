using eLibrary.BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eLibrary.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var book = await _bookService.GetBookByID(id);
                return View(book);
            }
            catch(Exception ex)
            {
                ViewBag.errorMessage = ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetByAuthorName(string authorName)
        {
            try
            {
                var books = await _bookService.GetByAuthorName(authorName);
                return View("Books", books);
            }
            catch(Exception ex)
            {
                ViewBag.errorMessage = ex.Message;
                return View("Error");
            }
        }


        public async Task<IActionResult> Books()
        {
            try
            {
                var books = await _bookService.GetBooks();
                return View(books);
            }
            catch(Exception ex)
            {
                ViewBag.errorMessage = ex.Message;
                return View("Error");
            }
        }
    }
}
