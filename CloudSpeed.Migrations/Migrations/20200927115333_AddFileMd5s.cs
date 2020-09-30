using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CloudSpeed.Migrations.Migrations
{
    public partial class AddFileMd5s : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dt_file_md5",
                columns: table => new
                {
                    id = table.Column<string>(maxLength: 36, nullable: false),
                    data_key = table.Column<string>(maxLength: 256, nullable: true),
                    created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dt_file_md5", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dt_file_md5");
        }
    }
}
