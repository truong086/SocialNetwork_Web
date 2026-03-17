using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class PostUserTag : BaseEntity
    {
        public int? post_id { get; set; }
        public int? user_id { get; set; }
        public Post? post { get; set; }
        public User? user { get; set; }
    }
}
