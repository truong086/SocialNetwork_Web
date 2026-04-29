using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Districts : BaseEntity
    {
        public string? name { get; set; }
        public int? city_id { get; set; }
        public Citys? citys { get; set; }
        //public virtual ICollection<Communes>? communes { get; set; }
        public ICollection<Communes>? communes { get; set; }
    }
}
