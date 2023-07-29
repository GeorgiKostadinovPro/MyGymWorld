using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGymWorld.Data.Migrations
{
    public partial class RemoveColumnsIsNotifiedByEmailAndBySMSFromTableUsersGyms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNotifiedByEmail",
                table: "UsersGyms");

            migrationBuilder.DropColumn(
                name: "IsNotifiedBySMS",
                table: "UsersGyms");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNotifiedByEmail",
                table: "UsersGyms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNotifiedBySMS",
                table: "UsersGyms",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
