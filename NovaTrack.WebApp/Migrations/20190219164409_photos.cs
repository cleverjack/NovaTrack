using Microsoft.EntityFrameworkCore.Migrations;

namespace NovaTrack.WebApp.Migrations
{
    public partial class photos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocalAssetPhoto",
                table: "Jobs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocalDevicePhoto",
                table: "Jobs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalAssetPhoto",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "LocalDevicePhoto",
                table: "Jobs");
        }
    }
}
