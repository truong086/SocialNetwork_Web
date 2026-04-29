using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Like : BaseEntity
    {
        public int? User_id { get; set; }
        public int? Post_id { get; set; }
        public int? icon_id { get; set; }
        public int? status { get; set; }
        public string? url_icon { get; set; }
        public User? user { get; set; }
        public Post? post { get; set; }
        public Icon? icon { get; set; }
    }
}
