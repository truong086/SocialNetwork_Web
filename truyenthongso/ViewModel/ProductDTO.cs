namespace truyenthongso.ViewModel
{
    public class ProductDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? Categoryid { get; set; }
        public int? Status { get; set; }
        public List<IFormFile>? images { get; set; }
        public List<int>? tagId { get; set; }
        public Tag_FriendDTO? tag_friendId { get; set; }
    }
}
