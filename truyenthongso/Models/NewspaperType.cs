using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class NewspaperType : BaseEntity
    {
        public string? Name { get; set; }
        public ICollection<NewspaperSource>? NewspaperSources { get; set; }
    }
}
