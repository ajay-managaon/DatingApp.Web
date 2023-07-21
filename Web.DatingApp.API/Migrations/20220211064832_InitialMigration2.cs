using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web.DatingApp.API.Migrations
{
    public partial class InitialMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "tbl_User",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "tbl_User",
                type: "varbinary(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "tbl_User");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "tbl_User");
        }
    }
}
