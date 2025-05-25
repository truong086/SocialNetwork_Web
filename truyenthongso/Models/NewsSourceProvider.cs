using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class NewsSourceProvider : BaseEntity
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Website { get; set; }
        public ICollection<NewspaperSource>? NewspaperSources { get; set; }
    }
}
