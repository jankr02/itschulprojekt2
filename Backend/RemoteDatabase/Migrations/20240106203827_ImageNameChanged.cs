using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemoteDatabase.Migrations
{
    /// <inheritdoc />
    public partial class ImageNameChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Data",
                table: "Pictures",
                newName: "Image");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Pictures",
                newName: "Data");
        }
    }
}
