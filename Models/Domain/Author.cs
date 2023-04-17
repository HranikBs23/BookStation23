namespace BookStation.Models.Domain
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Biography { get; set; }
        public string Image { get; set; }

        public List<Book> Books { get; set; }
    }
}
