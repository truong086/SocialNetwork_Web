using System.ComponentModel.DataAnnotations;

namespace truyenthongso.Common
{
    public class BaseEntity
    {
        protected BaseEntity() { }
        [Key]
        public int id { get; set; }
        public bool deleted { get; set; }
        public string? creator { get; set; }
        public DateTimeOffset? cretoredat { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? updateat { get; set; }
    }
}
