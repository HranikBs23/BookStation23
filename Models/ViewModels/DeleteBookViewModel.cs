﻿namespace BookStation.Models.ViewModels
{
    public class DeleteBookViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Language { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CoverImage { get; set; }
    }
}
