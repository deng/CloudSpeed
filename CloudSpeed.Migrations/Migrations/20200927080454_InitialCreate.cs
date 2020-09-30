using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CloudSpeed.Migrations.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dt_file_cid",
                columns: table => new
                {
                    id = table.Column<string>(maxLength: 36, nullable: false),
                    name = table.Column<string>(maxLength: 512, nullable: true),
                    status = table.Column<byte>(nullable: false),
                    created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dt_file_cid", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dt_file_name",
                columns: table => new
                {
                    id = table.Column<string>(maxLength: 36, nullable: false),
                    name = table.Column<string>(maxLength: 1024, nullable: true),
                    created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dt_file_name", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dt_upload_log",
                columns: table => new
                {
                    id = table.Column<string>(maxLength: 36, nullable: false),
                    description = table.Column<string>(nullable: true),
                    data_key = table.Column<string>(maxLength: 256, nullable: true),
                    alipay_file_key = table.Column<string>(maxLength: 256, nullable: true),
                    wxpay_file_key = table.Column<string>(maxLength: 256, nullable: true),
                    hashed_password = table.Column<string>(maxLength: 256, nullable: true),
                    created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dt_upload_log", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dt_file_cid");

            migrationBuilder.DropTable(
                name: "dt_file_name");

            migrationBuilder.DropTable(
                name: "dt_upload_log");
        }
    }
}
