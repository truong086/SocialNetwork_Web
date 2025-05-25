using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Group : BaseEntity
    {
        public string? Name { get; set; }
        public ICollection<Group_Role>? Group_Roles { get; set; }
    }
}
