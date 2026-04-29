using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Icon : BaseEntity
    {
        public string? name { get; set; }
        public string? url { get; set; }
        public int? user_id { get; set; }
        public User? user { get; set; }
        public ICollection<Like>? likes { get; set; }
    }
}
