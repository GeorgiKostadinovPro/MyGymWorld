using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGymWorld.Data.Migrations
{
    public partial class RemoveColumnIsSubscribedForEventsFromTableUsersGyms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSubscribedForEvents",
                table: "UsersGyms");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSubscribedForEvents",
                table: "UsersGyms",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
