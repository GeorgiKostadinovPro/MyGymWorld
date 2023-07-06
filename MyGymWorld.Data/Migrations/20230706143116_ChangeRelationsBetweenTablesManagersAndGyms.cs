using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGymWorld.Data.Migrations
{
    public partial class ChangeRelationsBetweenTablesManagersAndGyms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Managers_Gyms_GymId",
                table: "Managers");

            migrationBuilder.DropIndex(
                name: "IX_Managers_GymId",
                table: "Managers");

            migrationBuilder.DropColumn(
                name: "GymId",
                table: "Managers");

            migrationBuilder.AddColumn<Guid>(
                name: "ManagerId",
                table: "Gyms",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Gyms_ManagerId",
                table: "Gyms",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gyms_Managers_ManagerId",
                table: "Gyms",
                column: "ManagerId",
                principalTable: "Managers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gyms_Managers_ManagerId",
                table: "Gyms");

            migrationBuilder.DropIndex(
                name: "IX_Gyms_ManagerId",
                table: "Gyms");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Gyms");

            migrationBuilder.AddColumn<Guid>(
                name: "GymId",
                table: "Managers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Managers_GymId",
                table: "Managers",
                column: "GymId");

            migrationBuilder.AddForeignKey(
                name: "FK_Managers_Gyms_GymId",
                table: "Managers",
                column: "GymId",
                principalTable: "Gyms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
