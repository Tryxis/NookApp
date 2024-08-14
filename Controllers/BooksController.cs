using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
//using NookApp.Data;
using NookApp.Services;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace NookApp.Controllers
{
    public class BooksController : Controller
    {
        private readonly OpenLibraryService _openLibraryService;

        public BooksController(OpenLibraryService openLibraryService)
        {
            _openLibraryService = openLibraryService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(string isbn, string title, string author, int page, int pageSize)
        {
            page = 1;
            pageSize = 8;

            if (!string.IsNullOrEmpty(isbn))
            {
                var bookInfo = await GetBookInfoByIsbnAsync(isbn);
                return View("BookDetails", bookInfo);
            }
            else if (!string.IsNullOrEmpty(title))
            {
                var bookInfo = await GetBookInfoByTitleAsync(title);
                return View("BookDetails", bookInfo);
            }
            else if (!string.IsNullOrEmpty(author))
            {
                var books = await GetBooksByAuthorAsync(author);
                ViewBag.AuthorName = author;

                var paginatedBooks = books.ToPagedList(page, pageSize);
                return View("BooksList", paginatedBooks);
            }
            else
            {
                ViewBag.ErrorMessage = "Please enter at least one search criterion.";
                return View("Index");
            }
        }

        private async Task<JObject> GetBookInfoByIsbnAsync(string isbn)
        {
            var response = await _openLibraryService.GetBookInfoByIsbnAsync(isbn);
            return response.ContainsKey($"ISBN:{isbn}") ? response[$"ISBN:{isbn}"] as JObject : null;
        }

        private async Task<JObject> GetBookInfoByTitleAsync(string title)
        {
            var response = await _openLibraryService.GetBookInfoByTitleAsync(title);
            var docs = response["docs"] as JArray;
            return docs != null && docs.Count > 0 ? docs.FirstOrDefault() as JObject : null;
        }

        private async Task<JArray> GetBooksByAuthorAsync(string author)
        {
            var response = await _openLibraryService.GetBookInfoByAuthorAsync(author);
            var docs = response["docs"] as JArray;

            GetFirstPublishDate(docs);

            return docs ?? new JArray();
        }

        private void GetFirstPublishDate(JArray books)
        {
            if (books != null)
            {
                foreach (var book in books)
                {
                    var publishDates = (book["publish_date"] as JArray)?
                        .Select(d => DateTime.TryParse(d?.ToString(), out var dt) ? dt : (DateTime?)null)
                        .Where(d => d.HasValue)
                        .Select(d => d.Value);

                    if (publishDates != null && publishDates.Any())
                    {
                        var oldestPublishDate = publishDates.OrderBy(d => d).FirstOrDefault();
                        book["oldest_publish_date"] = oldestPublishDate.ToString("dd-MMM-yyyy");
                    }
                }
            }
        }
    }
}
