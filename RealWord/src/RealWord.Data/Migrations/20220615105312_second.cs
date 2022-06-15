using Microsoft.EntityFrameworkCore.Migrations;

namespace RealWord.Data.Migrations
{
    public partial class second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleFavorites_Articles_ArticleId",
                table: "ArticleFavorites");

            migrationBuilder.DropForeignKey(
                name: "FK_ArticleFavorites_Users_UserId",
                table: "ArticleFavorites");

            migrationBuilder.DropForeignKey(
                name: "FK_ArticleTags_Articles_ArticleId",
                table: "ArticleTags");

            migrationBuilder.DropForeignKey(
                name: "FK_ArticleTags_Tags_TagId",
                table: "ArticleTags");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleFavorites_Articles_ArticleId",
                table: "ArticleFavorites",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "ArticleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleFavorites_Users_UserId",
                table: "ArticleFavorites",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleTags_Articles_ArticleId",
                table: "ArticleTags",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "ArticleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleTags_Tags_TagId",
                table: "ArticleTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "TagId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleFavorites_Articles_ArticleId",
                table: "ArticleFavorites");

            migrationBuilder.DropForeignKey(
                name: "FK_ArticleFavorites_Users_UserId",
                table: "ArticleFavorites");

            migrationBuilder.DropForeignKey(
                name: "FK_ArticleTags_Articles_ArticleId",
                table: "ArticleTags");

            migrationBuilder.DropForeignKey(
                name: "FK_ArticleTags_Tags_TagId",
                table: "ArticleTags");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleFavorites_Articles_ArticleId",
                table: "ArticleFavorites",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "ArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleFavorites_Users_UserId",
                table: "ArticleFavorites",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleTags_Articles_ArticleId",
                table: "ArticleTags",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "ArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleTags_Tags_TagId",
                table: "ArticleTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "TagId");
        }
    }
}
