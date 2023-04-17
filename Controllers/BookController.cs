using BookStation.Data;
using BookStation.Models.Domain;
using BookStation.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookStation.Controllers
{
    public class BookController : Controller
    {
        private readonly ILogger<BookController> _logger;
        private readonly BookStationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookController(ILogger<BookController> logger, BookStationDbContext context, IWebHostEnvironment env)
        {
            _logger = logger;
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            List<Book> books = _context.Books.Include(b => b.Author).ToList();
            return View(books);
        }

        public IActionResult Add()
        {
            AddBookViewModel model = new AddBookViewModel
            {
                Authors = _context.Authors.Select(a => new SelectListItem
                {
                    Value = a.AuthorId.ToString(),
                    Text = a.Name
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBookViewModel model)
        {

            ModelState.Remove("AuthorId");
            ModelState.Remove("Authors");

            if (ModelState.IsValid)
            {
                Book book = new Book
                {
                    Title = model.Title,
                    AuthorId = model.AuthorId,
                    Language = model.Language,
                    Description = model.Description,
                    Price = model.Price,
                    
                };

                // Check if a cover image file was uploaded
                if (model.CoverImage != null && model.CoverImage.Length > 0)
                {
                    // Get the file name and extension
                    var fileName = Path.GetFileName(model.CoverImage.FileName);
                    var fileExtension = Path.GetExtension(fileName);

                    // Generate a unique file name
                    var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                    // Set the image path to the wwwroot/images/books directory
                    var imagePath = Path.Combine(_env.WebRootPath, "images", "books", uniqueFileName);

                    // Create the directory if it does not exist
                    Directory.CreateDirectory(Path.GetDirectoryName(imagePath));

                    // Save the image file to the server
                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await model.CoverImage.CopyToAsync(fileStream);
                    }

                    // Set the book's cover image property to the file path
                    book.CoverImage = "/images/books/" + uniqueFileName;
                }

                _context.Books.Add(book);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    // Do something with the error, e.g. log it or display it to the user
                    var errorMessage = error.ErrorMessage;
                    var exception = error.Exception;
                    _logger.LogInformation(errorMessage, exception);
                    // ...
                }
            }

            model.Authors = _context.Authors.Select(a => new SelectListItem
            {
                Value = a.AuthorId.ToString(),
                Text = a.Name
            }).ToList();

            return View(model);
        }
        public IActionResult Edit(int id)
        {
            var book = _context.Books.Include(b => b.Author).FirstOrDefault(b => b.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            var model = new EditBookViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                AuthorId = book.AuthorId,
                Language = book.Language,
                Description = book.Description,
                Price = book.Price,
                ExistingCoverImage = book.CoverImage,
                Authors = _context.Authors.Select(a => new SelectListItem
                {
                    Value = a.AuthorId.ToString(),
                    Text = a.Name
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditBookViewModel model)
        {
            if (ModelState.IsValid)
            {
                var book = _context.Books.FirstOrDefault(b => b.BookId == model.BookId);

                if (book == null)
                {
                    return NotFound();
                }

                book.Title = model.Title;
                book.AuthorId = model.AuthorId;
                book.Language = model.Language;
                book.Description = model.Description;
                book.Price = model.Price;

                // Check if a cover image file was uploaded
                if (model.NewCoverImage != null && model.NewCoverImage.Length > 0)
                {
                    // Get the file name and extension
                    var fileName = Path.GetFileName(model.NewCoverImage.FileName);
                    var fileExtension = Path.GetExtension(fileName);

                    // Generate a unique file name
                    var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                    // Set the image path to the wwwroot/images/books directory
                    var imagePath = Path.Combine(_env.WebRootPath, "images", "books", uniqueFileName);

                    // Create the directory if it does not exist
                    Directory.CreateDirectory(Path.GetDirectoryName(imagePath));

                    // Save the image file to the server
                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await model.NewCoverImage.CopyToAsync(fileStream);
                    }

                    // Set the book's cover image property to the file path
                    book.CoverImage = "/images/books/" + uniqueFileName;

                    // If an existing cover image exists, delete it from the server
                    if (!string.IsNullOrEmpty(model.ExistingCoverImage))
                    {
                        var existingImagePath = Path.Combine(_env.WebRootPath, model.ExistingCoverImage.TrimStart('/'));

                        if (System.IO.File.Exists(existingImagePath))
                        {
                            System.IO.File.Delete(existingImagePath);
                        }
                    }
                }

                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            model.Authors = _context.Authors.Select(a => new SelectListItem
            {
                Value = a.AuthorId.ToString(),
                Text = a.Name
            }).ToList();

            return View(model);
        }

    }
}
