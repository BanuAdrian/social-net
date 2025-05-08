using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace social_net.Migrations
{
    /// <inheritdoc />
    public partial class AddOnDeleteCascadeOnPhotoComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhotoComments_Photos_PhotoId",
                table: "PhotoComments");

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoComments_Photos_PhotoId",
                table: "PhotoComments",
                column: "PhotoId",
                principalTable: "Photos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhotoComments_Photos_PhotoId",
                table: "PhotoComments");

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoComments_Photos_PhotoId",
                table: "PhotoComments",
                column: "PhotoId",
                principalTable: "Photos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
