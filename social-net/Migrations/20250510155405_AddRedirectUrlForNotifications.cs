using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace social_net.Migrations
{
    /// <inheritdoc />
    public partial class AddRedirectUrlForNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RedirectUrl",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RedirectUrl",
                table: "Notifications");
        }
    }
}
