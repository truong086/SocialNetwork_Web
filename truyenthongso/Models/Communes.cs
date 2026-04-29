using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Communes : BaseEntity
    {
        public string? name { get; set; }   
        public int? district_id { get; set; }
        public Districts? districts { get; set; }
        public int? city_id { get; set; }   
        public Citys? citys { get; set; }
    }
}
