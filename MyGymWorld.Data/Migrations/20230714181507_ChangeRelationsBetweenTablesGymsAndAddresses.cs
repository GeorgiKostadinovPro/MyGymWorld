using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGymWorld.Data.Migrations
{
    public partial class ChangeRelationsBetweenTablesGymsAndAddresses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GymAddresses");

            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                table: "Gyms",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Gyms_AddressId",
                table: "Gyms",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gyms_Addresses_AddressId",
                table: "Gyms",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gyms_Addresses_AddressId",
                table: "Gyms");

            migrationBuilder.DropIndex(
                name: "IX_Gyms_AddressId",
                table: "Gyms");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Gyms");

            migrationBuilder.CreateTable(
                name: "GymAddresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GymId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GymAddresses_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GymAddresses_Gyms_GymId",
                        column: x => x.GymId,
                        principalTable: "Gyms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GymAddresses_AddressId",
                table: "GymAddresses",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_GymAddresses_GymId",
                table: "GymAddresses",
                column: "GymId");
        }
    }
}
