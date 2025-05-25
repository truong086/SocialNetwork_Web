using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Rating : BaseEntity
    {
        public int? Start { get; set; }
        public int? User_id { get; set; }
        public User? user{ get; set; }
    }
}
