using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Tag_Post : BaseEntity
    {
        public int? Tag_id { get; set; }
        public int? Post_id { get; set; }
        public Tag? tag { get; set; }
        public Post? post { get; set; }
    }
}
