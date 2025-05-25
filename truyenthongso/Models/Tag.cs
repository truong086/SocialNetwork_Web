using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Tag : BaseEntity
    {
        public string? Name { get; set; }
        public ICollection<Tag_Post>? TagPosts { get; set; }
    }
}
