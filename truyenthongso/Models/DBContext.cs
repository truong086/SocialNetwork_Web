using Microsoft.EntityFrameworkCore;

namespace truyenthongso.Models
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        #region
        public DbSet<Role> roles { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Comment> comments { get; set; }
        public DbSet<Complaints> complaints { get; set; }
        public DbSet<Group> groups { get; set; }
        public DbSet<Group_Role> groupRoles { get; set; }
        public DbSet<Interest> interests { get; set; }
        public DbSet<Like> likes { get; set; }
        public DbSet<NewspaperSource> newspaperSources { get; set; }
        public DbSet<NewspaperType> newspaperTypes { get; set; }
        public DbSet<NewsSourceProvider> newsSourceProviders { get; set; }
        public DbSet<Notifition> notifitions { get; set; }
        public DbSet<Post> posts { get; set; }
        public DbSet<Post_Image> postImages { get; set; }
        public DbSet<Rating> ratings { get; set; }
        public DbSet<Shere> sheres { get; set; }
        public DbSet<Tag> tags { get; set; }
        public DbSet<Tag_Post> tagPosts { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Token> tokens { get; set; }
        public DbSet<Friendship> friendships { get; set; }
        public DbSet<UserStoryView> userStoryViews { get; set; }
        public DbSet<PostUserTag> postUserTags { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(p => p.Interests)
                .WithOne(u => u.user)
                .HasForeignKey(m => m.User_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(p => p.postUserTagss)
                .WithOne(u => u.user)
                .HasForeignKey(m => m.user_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(p => p.Notifitions)
                .WithOne(u => u.user)
                .HasForeignKey(m => m.Use_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(p => p.Posts)
                .WithOne(u => u.user)
                .HasForeignKey(m => m.User_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(p => p.Complaintss)
                .WithOne(u => u.user)
                .HasForeignKey(m => m.User_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(p => p.Sheres)
                .WithOne(u => u.user)
                .HasForeignKey(m => m.User_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(p => p.Likes)
                .WithOne(u => u.user)
                .HasForeignKey(m => m.User_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(p => p.Ratings)
                .WithOne(u => u.user)
                .HasForeignKey(m => m.User_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
               .HasMany(p => p.Comments)
               .WithOne(u => u.user)
               .HasForeignKey(m => m.User_id)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
               .HasMany(p => p.CommentDescriptions)
               .WithOne(u => u.user)
               .HasForeignKey(m => m.User_id)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
              .HasMany(p => p.Friendships1)
              .WithOne(u => u.user1)
              .HasForeignKey(m => m.UserId1)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
              .HasMany(p => p.Friendships2)
              .WithOne(u => u.user2)
              .HasForeignKey(m => m.UserId2)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
               .HasMany(p => p.tokens)
               .WithOne(u => u.user)
               .HasForeignKey(m => m.userid)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
              .HasMany(p => p.userStoryViews)
              .WithOne(u => u.user)
              .HasForeignKey(m => m.user_id)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>()
               .HasMany(p => p.Group_Roles)
               .WithOne(u => u.role)
               .HasForeignKey(m => m.role_id)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>()
              .HasMany(p => p.Users)
              .WithOne(u => u.role)
              .HasForeignKey(m => m.role_id)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
              .HasMany(p => p.Posts)
              .WithOne(u => u.category)
              .HasForeignKey(m => m.Category_id)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
              .HasMany(p => p.CommentDescriptions)
              .WithOne(u => u.comment)
              .HasForeignKey(m => m.Comment_id)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Group>()
              .HasMany(p => p.Group_Roles)
              .WithOne(u => u.group)
              .HasForeignKey(m => m.group_id)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NewspaperType>()
              .HasMany(p => p.NewspaperSources)
              .WithOne(u => u.newspaperType)
              .HasForeignKey(m => m.NewspaperType_id)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NewsSourceProvider>()
              .HasMany(p => p.NewspaperSources)
              .WithOne(u => u.newsSourceProvider)
              .HasForeignKey(m => m.NewsSourceProvider_id)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
             .HasMany(p => p.Sheres)
             .WithOne(u => u.post)
             .HasForeignKey(m => m.Post_id)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
             .HasMany(p => p.Likes)
             .WithOne(u => u.post)
             .HasForeignKey(m => m.Post_id)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
             .HasMany(p => p.postUserTagss)
             .WithOne(u => u.post)
             .HasForeignKey(m => m.post_id)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
             .HasMany(p => p.TagPosts)
             .WithOne(u => u.post)
             .HasForeignKey(m => m.Post_id)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
             .HasMany(p => p.Comments)
             .WithOne(u => u.post)
             .HasForeignKey(m => m.Post_id)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
            .HasMany(p => p.userStoryViews)
            .WithOne(u => u.post)
            .HasForeignKey(m => m.post_id)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Tag>()
             .HasMany(p => p.TagPosts)
             .WithOne(u => u.tag)
             .HasForeignKey(m => m.Tag_id)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CommentDescription>()
             .HasMany(p => p.commentDescriptions2)
             .WithOne(u => u.CommentDescription_id2)
             .HasForeignKey(m => m.CommentDescription_id)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CommentDescription>()
             .HasMany(p => p.commentDescriptions3)
             .WithOne(u => u.CommentDescription_id3)
             .HasForeignKey(m => m.CommentDescript3_2)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
