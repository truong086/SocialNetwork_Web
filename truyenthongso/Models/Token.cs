using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Token : BaseEntity
    {
        public int? userid { get; set; }
        public User? user { get; set; }
        public string? token { get; set; }
    }
}