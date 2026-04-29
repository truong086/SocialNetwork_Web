using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Nation : BaseEntity
    {
        public string? name { get; set; }
        public ICollection<Citys> citys { get; set; }
    }
}
