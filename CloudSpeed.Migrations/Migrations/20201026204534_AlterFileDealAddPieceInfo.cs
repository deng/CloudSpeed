using Microsoft.EntityFrameworkCore.Migrations;

namespace CloudSpeed.Migrations.Migrations
{
    public partial class AlterFileDealAddPieceInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "piece_cid",
                table: "dt_file_deal",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "piece_size",
                table: "dt_file_deal",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "piece_cid",
                table: "dt_file_deal");

            migrationBuilder.DropColumn(
                name: "piece_size",
                table: "dt_file_deal");
        }
    }
}
