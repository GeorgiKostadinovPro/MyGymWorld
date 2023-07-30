using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGymWorld.Data.Migrations
{
    public partial class AddColumnsQRCodeUriAndPublicIdToTableUsersMemberships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "UsersMemberships",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "QRCodeUri",
                table: "UsersMemberships",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "UsersMemberships");

            migrationBuilder.DropColumn(
                name: "QRCodeUri",
                table: "UsersMemberships");
        }
    }
}
