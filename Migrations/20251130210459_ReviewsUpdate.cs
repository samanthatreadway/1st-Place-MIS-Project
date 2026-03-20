using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fa25group23final.Migrations
{
    /// <inheritdoc />
    public partial class ReviewsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Books_BookID",
                table: "Reviews");

            migrationBuilder.AlterColumn<int>(
                name: "BookID",
                table: "Reviews",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Books_BookID",
                table: "Reviews",
                column: "BookID",
                principalTable: "Books",
                principalColumn: "BookID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Books_BookID",
                table: "Reviews");

            migrationBuilder.AlterColumn<int>(
                name: "BookID",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Books_BookID",
                table: "Reviews",
                column: "BookID",
                principalTable: "Books",
                principalColumn: "BookID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
