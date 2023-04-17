using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BookStation.Models.ViewModels
{
    public class AddBookViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Author")]
        public int AuthorId { get; set; }

        public List<SelectListItem> Authors { get; set; }

        [Required]
        public string Language { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Display(Name = "Cover Image")]
        public IFormFile CoverImage { get; set; }
    }
}
