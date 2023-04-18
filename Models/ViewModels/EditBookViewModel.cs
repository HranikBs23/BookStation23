using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BookStation.Models.ViewModels
{
    public class EditBookViewModel
    {
        public int BookId { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Author")]
        public int AuthorId { get; set; }

        [Required]
        [Display(Name = "Language")]
        public string Language { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Cover Image")]
        public IFormFile NewCoverImage { get; set; }

        public string ExistingCoverImage { get; set; }

        public List<SelectListItem> Authors { get; set; }
    }
}
