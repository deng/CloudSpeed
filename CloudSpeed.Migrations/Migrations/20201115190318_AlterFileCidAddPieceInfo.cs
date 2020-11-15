using Microsoft.EntityFrameworkCore.Migrations;

namespace CloudSpeed.Migrations.Migrations
{
    public partial class AlterFileCidAddPieceInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "piece_cid",
                table: "dt_file_cid",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "piece_size",
                table: "dt_file_cid",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "piece_cid",
                table: "dt_file_cid");

            migrationBuilder.DropColumn(
                name: "piece_size",
                table: "dt_file_cid");
        }
    }
}
