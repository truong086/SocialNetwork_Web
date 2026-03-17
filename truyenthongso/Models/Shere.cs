using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Shere : BaseEntity
    {
        public string? title { get; set; }
        public int? status { get; set; }
        public int? User_id { get; set; }
        public int? Post_id { get; set; }
        public User? user { get; set; }
        public Post? post { get; set; }
    }
}
