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
        public string? Commune { get; set; }
        public string? District { get; set; }
        public string? City { get; set; }
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
    }
}
