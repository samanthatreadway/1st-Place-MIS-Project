using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fa25group23final.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShippingConfigs",
                columns: table => new
                {
                    ShippingConfigID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstBookFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdditionalBookFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingConfigs", x => x.ShippingConfigID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingConfigs");
        }
    }
}
