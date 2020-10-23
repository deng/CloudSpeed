using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CloudSpeed.Migrations.Migrations
{
    public partial class AddSizeToFileName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "format",
                table: "dt_file_name",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "group_id",
                table: "dt_file_name",
                maxLength: 36,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "size",
                table: "dt_file_name",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "dt_file_group",
                columns: table => new
                {
                    id = table.Column<string>(maxLength: 36, nullable: false),
                    name = table.Column<string>(maxLength: 1024, nullable: true),
                    group_id = table.Column<string>(maxLength: 36, nullable: true),
                    created = table.Column<DateTime>(nullable: false),
                    updated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dt_file_group", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dt_file_group");

            migrationBuilder.DropColumn(
                name: "format",
                table: "dt_file_name");

            migrationBuilder.DropColumn(
                name: "group_id",
                table: "dt_file_name");

            migrationBuilder.DropColumn(
                name: "size",
                table: "dt_file_name");
        }
    }
}
