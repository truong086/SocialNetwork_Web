using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Interest : BaseEntity
    {
        public string? Name { get; set; }
        public int? User_id { get; set; }
        public User? user { get; set; }
    }
}
