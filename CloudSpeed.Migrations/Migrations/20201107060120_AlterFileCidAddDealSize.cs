using Microsoft.EntityFrameworkCore.Migrations;

namespace CloudSpeed.Migrations.Migrations
{
    public partial class AlterFileCidAddDealSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "deal_size",
                table: "dt_file_cid",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "payload_size",
                table: "dt_file_cid",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "deal_size",
                table: "dt_file_cid");

            migrationBuilder.DropColumn(
                name: "payload_size",
                table: "dt_file_cid");
        }
    }
}
