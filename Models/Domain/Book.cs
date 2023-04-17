namespace BookStation.Models.Domain
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public int AuthorId { get; set; }
        public string Language { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CoverImage { get; set; }

        public Author Author { get; set; }
    }
}
