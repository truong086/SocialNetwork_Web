using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class Post : BaseEntity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? Action { get; set; }
        public int? Views { get; set; }
        public float? AverageiewTime { get; set; }
        public int? User_id { get; set; }
        public int? Category_id { get; set; }
        public User? user { get; set; }
        public Category? category { get; set; }
        public ICollection<Post_Image>? Post_Images { get; set; }
        public ICollection<Shere>? Sheres { get; set; }
        public ICollection<Like>? Likes { get; set; }
        public ICollection<Tag_Post>? TagPosts { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<UserStoryView>? userStoryViews { get; set; }
        public ICollection<PostUserTag>? postUserTagss { get; set; }
        public ICollection<Articles_Viewed>? articles_Vieweds { get; set; }
    }
}
