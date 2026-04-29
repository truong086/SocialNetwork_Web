using truyenthongso.Models;

namespace truyenthongso.ViewModel
{
    public class PostResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string url_icon { get; set; }
        public IEnumerable<string> Image { get; set; }
        public int Like { get; set; }
        public int Tym { get; set; }
        public int Share { get; set; }
        public int Comment { get; set; }
        public string User { get; set; }
        public List<tagPostData> Tag { get; set; }
        public int UserId { get; set; }
        public int category_id { get; set; }
        public string category_name { get; set; }
        public string UserImage { get; set; }
        public bool isLike { get; set; }
        public bool deleted { get; set; }
        public DateTimeOffset? Date { get; set; }
    }

    public class tagPostData
    {
        public int Id { get; set; }
        public string name { get; set; }
    }

}
