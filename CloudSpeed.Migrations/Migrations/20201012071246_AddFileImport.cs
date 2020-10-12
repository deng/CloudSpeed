using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CloudSpeed.Migrations.Migrations
{
    public partial class AddFileImport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dt_file_import",
                columns: table => new
                {
                    id = table.Column<string>(maxLength: 36, nullable: false),
                    path = table.Column<string>(maxLength: 2048, nullable: true),
                    status = table.Column<byte>(nullable: false),
                    total = table.Column<long>(nullable: false),
                    success = table.Column<long>(nullable: false),
                    failed = table.Column<long>(nullable: false),
                    error = table.Column<string>(maxLength: 2048, nullable: true),
                    created = table.Column<DateTime>(nullable: false),
                    updated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dt_file_import", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dt_file_import");
        }
    }
}
