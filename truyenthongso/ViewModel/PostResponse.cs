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
        public List<tagPostData>? Tag_Friend { get; set; }
        public int UserId { get; set; }
        public int category_id { get; set; }
        public int? Status { get; set; }
        public string category_name { get; set; }
        public string UserImage { get; set; }
        public bool isLike { get; set; }
        public List<iconCount>? iconCounts { get; set; }
        public List<commentData>? comment_Data { get; set; }
        public bool deleted { get; set; }
        public DateTimeOffset? Date { get; set; }
    }

    public class iconCount
    {
        public string? icon { get; set; }
        public int count { get; set; }
    }
    public class tagPostData
    {
        public int Id { get; set; }
        public string name { get; set; }
    }

    public class commentData
    {
        public int id { get; set; }
        public int id_post { get; set; }
        public string? image_user { get; set; }
        public string? text { get; set; }
        public string? user_name { get; set; }
        public string? url { get; set; }
        public int total_CommentDescript { get; set; }
    }

}
