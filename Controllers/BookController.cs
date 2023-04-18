using BookStation.Data;
using BookStation.Models.Domain;
using BookStation.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;



namespace BookStation.Controllers
{
    // The BookController class inherits from the Controller class
    public class BookController : Controller
    {
        // The ILogger<BookController>, BookStationDbContext, and IWebHostEnvironment dependencies are injected via the constructor
        private readonly ILogger<BookController> _logger;
        private readonly BookStationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookController(ILogger<BookController> logger, BookStationDbContext context, IWebHostEnvironment env)
        {
            _logger = logger;
            _context = context;
            _env = env;
        }

        //The Index action method returns a view that displays a list of books and their authors
        public IActionResult Index()
        { 
            // The list of books with their corresponding authors is fetched from the database using the BookStationDbContext
            List<Book> books = _context.Books.Include(b => b.Author).ToList();
            // The view is returned with the list of books as its model
            return View(books);
        }

        // GET: Book/Add
        // Displays the form for adding a new book
        public IActionResult Add()
        {
            // Create a new view model and populate its authors list with data from the database
            AddBookViewModel model = new AddBookViewModel
            {
                Authors = _context.Authors.Select(a => new SelectListItem
                {
                    Value = a.AuthorId.ToString(),
                    Text = a.Name
                }).ToList()
            };
            // Render the view with the new view model
            return View(model);
        }


        // POST: Book/Add
        // Handles the submission of the form for adding a new book
        [HttpPost]
        public async Task<IActionResult> Add(AddBookViewModel model)
        {
            // Remove the AuthorId and Authors fields from the model state as they are not used in this action
            ModelState.Remove("AuthorId");
            ModelState.Remove("Authors");

            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Create a new book object and populate it with data from the view model
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
                // Add the book to the database and save changes
                _context.Books.Add(book);
                _context.SaveChanges();

                //// Redirect the user to the index action
                return RedirectToAction("Index");
            }
            else
            {
                // If the model state is invalid, log any errors and display them to the user
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    // Do something with the error, e.g. log it or display it to the user
                    var errorMessage = error.ErrorMessage;
                    var exception = error.Exception;
                    _logger.LogInformation(errorMessage, exception);
                    
                }
            }

            // If there were errors, repopulate the authors list and render the view with the same view model
            model.Authors = _context.Authors.Select(a => new SelectListItem
            {
                Value = a.AuthorId.ToString(),
                Text = a.Name
            }).ToList();

            return View(model);
        }
        // GET: Books/Edit/5
        public IActionResult Edit(int id)
        {
            // Retrieve the book with the specified id, including its Author property
            var book = _context.Books.Include(b => b.Author).FirstOrDefault(b => b.BookId == id);
            // If the book does not exist, we return a 404 Not Found response
            if (book == null)
            {
                return NotFound();
            }

            // Create a new EditBookViewModel and initialize its properties with the book's data

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
            // Pass the view model to the Edit view
            return View(model);
        }

        // POST: Books/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(EditBookViewModel model)
        {
          // Remove unnecessary properties from ModelState to avoid validation errors
           
            ModelState.Remove("AuthorId");

            // If the model state is valid, update the book's data and save the changes
            if (ModelState.IsValid)
            {
                // Retrieve the book with the specified id
                var book = _context.Books.FirstOrDefault(b => b.BookId == model.BookId);
                // If the book does not exist, return a 404 Not Found response
                if (book == null)
                {
                    return NotFound();
                }
                // Update the book's properties with the values from the view model
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

                    // Delete the existing cover image file
                    if (!string.IsNullOrEmpty(model.ExistingCoverImage))
                    {
                        var existingImagePath = Path.Combine(_env.WebRootPath, model.ExistingCoverImage.TrimStart('/'));

                        if (System.IO.File.Exists(existingImagePath))
                        {
                            System.IO.File.Delete(existingImagePath);
                        }
                    }

                    // Set the book's cover image property to the file path
                    book.CoverImage = "/images/books/" + uniqueFileName;
                }
                // Save the changes to the database
                _context.SaveChanges();
                // Redirect to the Index action
                return RedirectToAction("Index");
            }
            // If the model state is not valid, repopulate the Authors property 
            model.Authors = _context.Authors.Select(a => new SelectListItem
            {
                Value = a.AuthorId.ToString(),
                Text = a.Name
            }).ToList();

            return View(model);
        }
        public IActionResult Delete(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            var model = new DeleteBookViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                CoverImage = book.CoverImage
            };

            return View(model);
        }

        

    }
}
