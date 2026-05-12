using truyenthongso.Common;

namespace truyenthongso.Models
{
    public class User : BaseEntity
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public int Age { get; set; }
        public bool? Paythefee { get; set; }
        public bool Action { get; set; }
        public string? Image { get; set; }
        public string? PublicId { get; set; }
        public string? Commune { get; set; }
        public string? District { get; set; }
        public string? City { get; set; }
        public string? Nation { get; set; }
        public string? Address { get; set; }
        public int? Start { get; set; }
        public int? Complaints { get; set; }
        public bool? BlockComplaints { get; set; }
        public int? role_id { get; set; }
        public Role? role { get; set; }
        public ICollection<Interest>? Interests { get; set; }
        public ICollection<Notifition>? Notifitions { get; set; }
        public ICollection<Post>? Posts { get; set; }
        public ICollection<Complaints>? Complaintss { get; set; }
        public ICollection<Shere>? Sheres { get; set; }
        public ICollection<Like>? Likes { get; set; }
        public ICollection<Rating>? Ratings { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<CommentDescription>? CommentDescriptions { get; set; }
        public ICollection<Token>? tokens { get; set; }
        public ICollection<Friendship>? Friendships1 { get; set; }
        public ICollection<Friendship>? Friendships2 { get; set; }
        public ICollection<UserStoryView>? userStoryViews { get; set; }
        public ICollection<PostUserTag>? postUserTagss { get; set; }
        public ICollection<Articles_Viewed>? articles_Vieweds { get; set; }
        public ICollection<Behavioral_Analysis>? behavioral_Analyses { get; set; }
        public ICollection<Icon>? iconss { get; set; }
        public ICollection<Tag_Friend>? tag_Friends { get; set; }
        public ICollection<Tag_Friend>? friends { get; set; }
    }
}