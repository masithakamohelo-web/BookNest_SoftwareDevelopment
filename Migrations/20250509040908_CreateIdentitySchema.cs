using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASPNETCore_DB.Migrations
{
    /// <inheritdoc />
    public partial class CreateIdentitySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Condition",
                table: "Book");

            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "Book",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Book_BookId",
                table: "Book",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Book_BookId",
                table: "Book",
                column: "BookId",
                principalTable: "Book",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_Book_BookId",
                table: "Book");

            migrationBuilder.DropIndex(
                name: "IX_Book_BookId",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "Book");

            migrationBuilder.AddColumn<string>(
                name: "Condition",
                table: "Book",
                type: "TEXT",
                nullable: true);
        }
    }
}
