using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Category : BaseEntity
    {
        public string? Name { get; set; }
        public ICollection<Post>? Posts { get; set; }
    }
}
