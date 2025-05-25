using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class CommentDescription : BaseEntity
    {
        public int? User_id { get; set; }
        public int? Comment_id { get; set; }
        public Comment? comment { get; set; }
        public User? user { get; set; }
        public int? CommentDescription_id { get; set; }
        public int? CommentDescript3_2 { get; set; }
    }
}
