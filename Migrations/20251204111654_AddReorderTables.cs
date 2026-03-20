using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fa25group23final.Migrations
{
    /// <inheritdoc />
    public partial class AddReorderTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reorders",
                columns: table => new
                {
                    ReorderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reorders", x => x.ReorderID);
                });

            migrationBuilder.CreateTable(
                name: "ReorderDetails",
                columns: table => new
                {
                    ReorderDetailsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuantityOrdered = table.Column<int>(type: "int", nullable: false),
                    QuantityReceived = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReorderID = table.Column<int>(type: "int", nullable: false),
                    BookID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReorderDetails", x => x.ReorderDetailsID);
                    table.ForeignKey(
                        name: "FK_ReorderDetails_Books_BookID",
                        column: x => x.BookID,
                        principalTable: "Books",
                        principalColumn: "BookID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReorderDetails_Reorders_ReorderID",
                        column: x => x.ReorderID,
                        principalTable: "Reorders",
                        principalColumn: "ReorderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReorderDetails_BookID",
                table: "ReorderDetails",
                column: "BookID");

            migrationBuilder.CreateIndex(
                name: "IX_ReorderDetails_ReorderID",
                table: "ReorderDetails",
                column: "ReorderID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReorderDetails");

            migrationBuilder.DropTable(
                name: "Reorders");
        }
    }
}
