using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Friendship : BaseEntity
    {
        public int? UserId1 { get; set; }
        public int? UserId2 { get; set; }
        public int status { get; set; }
        public User? user1 { get; set; }
        public User? user2 { get; set; }
    }
}
