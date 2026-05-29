namespace truyenthongso.ViewModel
{
    public class CommentDTo
    {
        public int post_id {  get; set; }
        public string? description { get; set; }
        public IFormFile? file { get; set; }
    }
}
