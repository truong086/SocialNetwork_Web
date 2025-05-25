using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Role : BaseEntity
    {
        public string? Url { get; set; }
        public string? Description { get; set; }
        public ICollection<Group_Role>? Group_Roles { get; set; }
        public ICollection<User>? Users { get; set; }
    }
}
