using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CloudSpeed.Migrations.Migrations
{
    public partial class AddFileDeal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dt_file_deal",
                columns: table => new
                {
                    id = table.Column<string>(maxLength: 36, nullable: false),
                    name = table.Column<string>(maxLength: 512, nullable: true),
                    deal_id = table.Column<string>(maxLength: 512, nullable: true),
                    miner = table.Column<string>(maxLength: 18, nullable: true),
                    status = table.Column<byte>(nullable: false),
                    error = table.Column<string>(maxLength: 2048, nullable: true),
                    created = table.Column<DateTime>(nullable: false),
                    updated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dt_file_deal", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dt_file_deal");
        }
    }
}
