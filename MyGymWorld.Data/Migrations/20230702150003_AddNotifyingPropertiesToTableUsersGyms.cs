using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGymWorld.Data.Migrations
{
    public partial class AddNotifyingPropertiesToTableUsersGyms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManagersGyms");

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

            migrationBuilder.AddColumn<bool>(
                name: "IsSubscribedForEvents",
                table: "UsersGyms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "GymId",
                table: "Managers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "AddressId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Managers_Gyms_GymId",
                table: "Managers");

            migrationBuilder.DropIndex(
                name: "IX_Managers_GymId",
                table: "Managers");

            migrationBuilder.DropColumn(
                name: "IsNotifiedByEmail",
                table: "UsersGyms");

            migrationBuilder.DropColumn(
                name: "IsNotifiedBySMS",
                table: "UsersGyms");

            migrationBuilder.DropColumn(
                name: "IsSubscribedForEvents",
                table: "UsersGyms");

            migrationBuilder.DropColumn(
                name: "GymId",
                table: "Managers");

            migrationBuilder.AlterColumn<Guid>(
                name: "AddressId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ManagersGyms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GymId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagersGyms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagersGyms_Gyms_GymId",
                        column: x => x.GymId,
                        principalTable: "Gyms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManagersGyms_Managers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Managers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ManagersGyms_GymId",
                table: "ManagersGyms",
                column: "GymId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagersGyms_ManagerId",
                table: "ManagersGyms",
                column: "ManagerId");
        }
    }
}
