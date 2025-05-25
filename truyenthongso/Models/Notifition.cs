using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Notifition : BaseEntity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? Use_id { get; set; }
        public User? user { get; set; }

    }
}
