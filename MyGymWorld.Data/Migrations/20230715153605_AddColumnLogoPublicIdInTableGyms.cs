using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGymWorld.Data.Migrations
{
    public partial class AddColumnLogoPublicIdInTableGyms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoPublicId",
                table: "Gyms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoPublicId",
                table: "Gyms");
        }
    }
}
