using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OriginalFilePath",
                table: "Jobs",
                newName: "OriginalWebPath");

            migrationBuilder.AddColumn<string>(
                name: "OriginalSystemPath",
                table: "Jobs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalSystemPath",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "OriginalWebPath",
                table: "Jobs",
                newName: "OriginalFilePath");
        }
    }
}
