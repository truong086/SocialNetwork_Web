using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Tag_Friend : BaseEntity
    {
        public int? post_id { get; set; }
        public int? user_id { get; set; }   
        public int? friend_id { get; set; }
        public Post? post { get; set; }
        public User? user { get; set; }
        public User? friend { get; set; }
    }
}
