namespace BookStation.Models.ViewModels
{
    public class DeleteAuthorViewModel
    {
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Biography { get; set; }
        public string Image { get; set; }
        public List<BookViewModel> Books { get; set; }
    }

    public class BookViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; }
    }
}
