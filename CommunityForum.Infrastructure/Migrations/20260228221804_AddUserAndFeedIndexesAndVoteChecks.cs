using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunityForum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAndFeedIndexesAndVoteChecks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Topics_CategoryId",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Posts_TopicId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Comments_PostId",
                table: "Comments");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Votes_ExactlyOneTarget",
                table: "Votes",
                sql: "(([PostId] IS NOT NULL AND [CommentId] IS NULL) OR ([PostId] IS NULL AND [CommentId] IS NOT NULL))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Votes_VoteType_Valid",
                table: "Votes",
                sql: "[VoteType] IN (0, 1)");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Topics_CategoryId_CreatedAt",
                table: "Topics",
                columns: new[] { "CategoryId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_TopicId_CreatedAt",
                table: "Posts",
                columns: new[] { "TopicId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId_CreatedAt",
                table: "Comments",
                columns: new[] { "PostId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Votes_ExactlyOneTarget",
                table: "Votes");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Votes_VoteType_Valid",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Topics_CategoryId_CreatedAt",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Posts_TopicId_CreatedAt",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Comments_PostId_CreatedAt",
                table: "Comments");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_CategoryId",
                table: "Topics",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_TopicId",
                table: "Posts",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");
        }
    }
}
