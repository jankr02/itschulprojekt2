namespace MesseauftrittDatenerfassung.Models
{
    public class Picture
    {
        public int Id { get; set; }
        public required string Data { get; set; }
        public Customer? Customer { get; set; }
    }
}
