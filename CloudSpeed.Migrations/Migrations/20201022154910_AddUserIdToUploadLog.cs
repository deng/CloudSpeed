using Microsoft.EntityFrameworkCore.Migrations;

namespace CloudSpeed.Migrations.Migrations
{
    public partial class AddUserIdToUploadLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "user_id",
                table: "dt_upload_log",
                maxLength: 36,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_dt_upload_log_user_id",
                table: "dt_upload_log",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_dt_upload_log_user_id",
                table: "dt_upload_log");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "dt_upload_log");
        }
    }
}
