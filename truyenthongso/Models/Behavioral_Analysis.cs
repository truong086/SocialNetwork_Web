using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Behavioral_Analysis : BaseEntity
    {
        public decimal? total { get; set; }
        public int? category_id { get; set; }
        public Category? category { get; set; }
        public int? user_id { get; set; }
        public User? user { get; set; }
    }
}
