namespace truyenthongso.ViewModel
{
    public class ProductDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? Categoryid { get; set; }
        public List<IFormFile>? images { get; set; }
        public List<int>? tagId { get; set; }
    }
}
