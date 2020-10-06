using Microsoft.EntityFrameworkCore.Migrations;

namespace CloudSpeed.Migrations.Migrations
{
    public partial class AlterFileJobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "error",
                table: "dt_file_job",
                maxLength: 2048,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "error",
                table: "dt_file_job");
        }
    }
}
