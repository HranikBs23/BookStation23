using System.ComponentModel.DataAnnotations;

namespace BookStation.Models.ViewModels
{
    public class AddAuthorViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Biography { get; set; }

        public IFormFile Image { get; set; }
    }
}
