using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Complaints : BaseEntity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? Action { get; set; }
        public string? Url { get; set; }
        public string? PublicId { get; set; }
        public int? Author { get; set; }
        public int? User_id { get; set; }
        public User? user { get; set; }
    }
}
