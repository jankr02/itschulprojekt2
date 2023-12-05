namespace MesseauftrittDatenerfassung.Models
{
    public class Picture
    {
        public int Id { get; set; } = 1;
        public byte[]? Data { get; set; }
        public Customer? Customer { get; set; }
    }
}
