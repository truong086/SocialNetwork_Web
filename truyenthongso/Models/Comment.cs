using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Comment : BaseEntity
    {
        public int? Post_id { get; set; }
        public int? User_id { get; set; }
        public User? user { get; set; }
        public Post? post { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
        public string? PublicId_id { get; set; }
        public ICollection<CommentDescription>? CommentDescriptions { get; set; }
    }
}
