using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class UserStoryView : BaseEntity
    {
        public int? user_id { get; set; }
        public int? post_id { get; set; }
        public User? user { get; set; } 
        public Post? post { get; set; }
    }
}
