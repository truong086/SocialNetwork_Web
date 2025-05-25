using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Post_Image : BaseEntity
    {
        public string? Url { get; set; }
        public string? PublicId { get; set; }
        public int? Post_id { get; set; }
        public Post? post { get; set; }
    }
}
