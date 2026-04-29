using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Citys : BaseEntity
    {
        public string? name { get; set; }
        public int? nation_id { get; set; }
        public Nation? nation { get; set; }
        public ICollection<Communes>? communes { get; set; }
        public ICollection<Districts>? districts { get; set; }
    }
}
