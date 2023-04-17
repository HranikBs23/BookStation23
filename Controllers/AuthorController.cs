using BookStation.Data;
using BookStation.Models.Domain;
using BookStation.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookStation.Controllers
{
    public class AuthorController : Controller
    {
        private readonly ILogger<AuthorController> _logger;
        private readonly BookStationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AuthorController(ILogger<AuthorController> logger, BookStationDbContext context, IWebHostEnvironment env)
        {
            _logger = logger;
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            List<Author> authors = _context.Authors.ToList();
            foreach (var author in authors)
            {
                author.Books = _context.Books.Where(b => b.AuthorId == author.AuthorId).ToList();
            }
            return View(authors);
        }

        public IActionResult Add()
        {
            return View(new AddAuthorViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddAuthorViewModel model)
        {

            ModelState.Remove("AuthorId");
            if (ModelState.IsValid)
            {
                Author author = new Author
                {
                    Name = model.Name,
                    Address = model.Address,
                    Biography = model.Biography
                };

                // Check if an image file was uploaded
                if (model.Image != null && model.Image.Length > 0)
                {
                    // Get the file name and extension
                    var fileName = Path.GetFileName(model.Image.FileName);
                    var fileExtension = Path.GetExtension(fileName);

                    // Generate a unique file name
                    var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                    // Set the image path to the wwwroot/images/authors directory
                    var imagePath = Path.Combine(_env.WebRootPath, "images", "authors", uniqueFileName);

                    // Create the directory if it does not exist
                    Directory.CreateDirectory(Path.GetDirectoryName(imagePath));

                    // Save the image file to the server
                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(fileStream);
                    }

                    // Set the author's image property to the file path
                    author.Image = "/images/authors/" + uniqueFileName;
                }

                _context.Authors.Add(author);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            

            return View(model);
        }
        public IActionResult Edit(int id)
        {
            Author author = _context.Authors.FirstOrDefault(a => a.AuthorId == id);
            if (author == null)
            {
                return NotFound();
            }

            // Create an instance of the EditAuthorViewModel and populate it with the
            // data from the Author entity
            EditAuthorViewModel viewModel = new EditAuthorViewModel
            {
                AuthorId = author.AuthorId,
                Name = author.Name,
                Address = author.Address,
                Biography = author.Biography
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditAuthorViewModel model)
        {
            if (ModelState.IsValid)
            {
                Author author = _context.Authors.FirstOrDefault(a => a.AuthorId == model.AuthorId);
                if (author == null)
                {
                    return NotFound();
                }

                // Update the Author entity with the data from the EditAuthorViewModel
                author.Name = model.Name;
                author.Address = model.Address;
                author.Biography = model.Biography;

                // Check if an image file was uploaded
                if (model.Image != null && model.Image.Length > 0)
                {
                    // Get the file name and extension
                    var fileName = Path.GetFileName(model.Image.FileName);
                    var fileExtension = Path.GetExtension(fileName);

                    // Generate a unique file name
                    var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                    // Set the image path to the wwwroot/images/authors directory
                    var imagePath = Path.Combine(_env.WebRootPath, "images", "authors", uniqueFileName);

                    // Create the directory if it does not exist
                    Directory.CreateDirectory(Path.GetDirectoryName(imagePath));

                    // Save the image file to the server
                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(fileStream);
                    }

                    // Set the author's image property to the file path
                    author.Image = "/images/authors/" + uniqueFileName;
                }

                _context.Authors.Update(author);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            var books = _context.Books.Where(b => b.AuthorId == id);

            _context.Books.RemoveRange(books);
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



    }
}


