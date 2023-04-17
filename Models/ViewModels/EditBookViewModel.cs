using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BookStation.Models.ViewModels
{
    public class EditBookViewModel
    {
        public int BookId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public int AuthorId { get; set; }
        [Required]
        public string Language { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        public IFormFile NewCoverImage { get; set; }

        public string ExistingCoverImage { get; set; }
        public List<SelectListItem> Authors { get; set; }
    }
}
