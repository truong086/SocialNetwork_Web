using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class CommentDescription : BaseEntity
    {
        public int? User_id { get; set; }
        public int? Comment_id { get; set; }
        public string? Url { get; set; }
        public string? PublicId { get; set; }
        public string? descript { get; set; }
        public Comment? comment { get; set; }
        public User? user { get; set; }
        public int? CommentDescription_id { get; set; }
        public CommentDescription? CommentDescription_id2 { get; set; }
        public int? CommentDescript3_2 { get; set; }
        public CommentDescription? CommentDescription_id3 { get; set; }

        public ICollection<CommentDescription>? commentDescriptions2 { get; set; }
        public ICollection<CommentDescription>? commentDescriptions3 { get; set; }
    }
}
