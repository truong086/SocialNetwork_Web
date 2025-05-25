using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace truyenthongso.Migrations
{
    public partial class dbs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "newspaperTypes",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_newspaperTypes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "newsSourceProviders",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Address = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Website = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_newsSourceProviders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Url = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "newspaperSources",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NewspaperType_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    NewsSourceProvider_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Url = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Status = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_newspaperSources", x => x.id);
                    table.ForeignKey(
                        name: "FK_newspaperSources_newspaperTypes_NewspaperType_id",
                        column: x => x.NewspaperType_id,
                        principalTable: "newspaperTypes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_newspaperSources_newsSourceProviders_NewsSourceProvider_id",
                        column: x => x.NewsSourceProvider_id,
                        principalTable: "newsSourceProviders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "groupRoles",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    group_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    role_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groupRoles", x => x.id);
                    table.ForeignKey(
                        name: "FK_groupRoles_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_groupRoles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    UserName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Password = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    FullName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Age = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Paythefee = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    Commune = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    District = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    City = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Address = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Start = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Complaints = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    BlockComplaints = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    role_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "complaints",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Title = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Action = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    Url = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PublicId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Author = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    User_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_complaints", x => x.id);
                    table.ForeignKey(
                        name: "FK_complaints_users_User_id",
                        column: x => x.User_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "interests",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    User_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_interests", x => x.id);
                    table.ForeignKey(
                        name: "FK_interests_users_User_id",
                        column: x => x.User_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notifitions",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Title = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Use_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifitions", x => x.id);
                    table.ForeignKey(
                        name: "FK_notifitions_users_Use_id",
                        column: x => x.Use_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "posts",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Title = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Action = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    Views = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    AverageiewTime = table.Column<float>(type: "BINARY_FLOAT", nullable: true),
                    User_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Category_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_posts", x => x.id);
                    table.ForeignKey(
                        name: "FK_posts_categories_Category_id",
                        column: x => x.Category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_posts_users_User_id",
                        column: x => x.User_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ratings",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Start = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    User_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ratings", x => x.id);
                    table.ForeignKey(
                        name: "FK_ratings_users_User_id",
                        column: x => x.User_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Post_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    User_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Url = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PublicId_id = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comments", x => x.id);
                    table.ForeignKey(
                        name: "FK_comments_posts_Post_id",
                        column: x => x.Post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_comments_users_User_id",
                        column: x => x.User_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "likes",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    User_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Post_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_likes", x => x.id);
                    table.ForeignKey(
                        name: "FK_likes_posts_Post_id",
                        column: x => x.Post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_likes_users_User_id",
                        column: x => x.User_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "postImages",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Url = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PublicId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Post_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    postid = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_postImages", x => x.id);
                    table.ForeignKey(
                        name: "FK_postImages_posts_postid",
                        column: x => x.postid,
                        principalTable: "posts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "sheres",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    User_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Post_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sheres", x => x.id);
                    table.ForeignKey(
                        name: "FK_sheres_posts_Post_id",
                        column: x => x.Post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sheres_users_User_id",
                        column: x => x.User_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tagPosts",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Tag_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Post_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tagPosts", x => x.id);
                    table.ForeignKey(
                        name: "FK_tagPosts_posts_Post_id",
                        column: x => x.Post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tagPosts_tags_Tag_id",
                        column: x => x.Tag_id,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommentDescription",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    User_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Comment_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    CommentDescription_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    CommentDescript3_2 = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentDescription", x => x.id);
                    table.ForeignKey(
                        name: "FK_CommentDescription_comments_Comment_id",
                        column: x => x.Comment_id,
                        principalTable: "comments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentDescription_users_User_id",
                        column: x => x.User_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentDescription_Comment_id",
                table: "CommentDescription",
                column: "Comment_id");

            migrationBuilder.CreateIndex(
                name: "IX_CommentDescription_User_id",
                table: "CommentDescription",
                column: "User_id");

            migrationBuilder.CreateIndex(
                name: "IX_comments_Post_id",
                table: "comments",
                column: "Post_id");

            migrationBuilder.CreateIndex(
                name: "IX_comments_User_id",
                table: "comments",
                column: "User_id");

            migrationBuilder.CreateIndex(
                name: "IX_complaints_User_id",
                table: "complaints",
                column: "User_id");

            migrationBuilder.CreateIndex(
                name: "IX_groupRoles_group_id",
                table: "groupRoles",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_groupRoles_role_id",
                table: "groupRoles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_interests_User_id",
                table: "interests",
                column: "User_id");

            migrationBuilder.CreateIndex(
                name: "IX_likes_Post_id",
                table: "likes",
                column: "Post_id");

            migrationBuilder.CreateIndex(
                name: "IX_likes_User_id",
                table: "likes",
                column: "User_id");

            migrationBuilder.CreateIndex(
                name: "IX_newspaperSources_NewspaperType_id",
                table: "newspaperSources",
                column: "NewspaperType_id");

            migrationBuilder.CreateIndex(
                name: "IX_newspaperSources_NewsSourceProvider_id",
                table: "newspaperSources",
                column: "NewsSourceProvider_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifitions_Use_id",
                table: "notifitions",
                column: "Use_id");

            migrationBuilder.CreateIndex(
                name: "IX_postImages_postid",
                table: "postImages",
                column: "postid");

            migrationBuilder.CreateIndex(
                name: "IX_posts_Category_id",
                table: "posts",
                column: "Category_id");

            migrationBuilder.CreateIndex(
                name: "IX_posts_User_id",
                table: "posts",
                column: "User_id");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_User_id",
                table: "ratings",
                column: "User_id");

            migrationBuilder.CreateIndex(
                name: "IX_sheres_Post_id",
                table: "sheres",
                column: "Post_id");

            migrationBuilder.CreateIndex(
                name: "IX_sheres_User_id",
                table: "sheres",
                column: "User_id");

            migrationBuilder.CreateIndex(
                name: "IX_tagPosts_Post_id",
                table: "tagPosts",
                column: "Post_id");

            migrationBuilder.CreateIndex(
                name: "IX_tagPosts_Tag_id",
                table: "tagPosts",
                column: "Tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                table: "users",
                column: "role_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentDescription");

            migrationBuilder.DropTable(
                name: "complaints");

            migrationBuilder.DropTable(
                name: "groupRoles");

            migrationBuilder.DropTable(
                name: "interests");

            migrationBuilder.DropTable(
                name: "likes");

            migrationBuilder.DropTable(
                name: "newspaperSources");

            migrationBuilder.DropTable(
                name: "notifitions");

            migrationBuilder.DropTable(
                name: "postImages");

            migrationBuilder.DropTable(
                name: "ratings");

            migrationBuilder.DropTable(
                name: "sheres");

            migrationBuilder.DropTable(
                name: "tagPosts");

            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "newspaperTypes");

            migrationBuilder.DropTable(
                name: "newsSourceProviders");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "posts");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
