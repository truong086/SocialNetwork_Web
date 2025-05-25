using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class NewspaperSource : BaseEntity
    {
        public string? Name {  get; set; }
        public int? NewspaperType_id {  get; set; }
        public int? NewsSourceProvider_id {  get; set; }
        public NewspaperType? newspaperType {  get; set; }
        public NewsSourceProvider? newsSourceProvider {  get; set; }
        public string? Url {  get; set; }
        public bool? Status {  get; set; }
        public string? Description {  get; set; }
    }
}
