namespace truyenthongso.ViewModel
{
    public class PostResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> Image { get; set; }
        public int Like { get; set; }
        public int Tym { get; set; }
        public int Share { get; set; }
        public int Comment { get; set; }
        public string User { get; set; }
        public int UserId { get; set; }
        public string UserImage { get; set; }
        public DateTimeOffset? Date { get; set; }
    }

}
