using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elearning_Test.Migrations
{
    /// <inheritdoc />
    public partial class NomDeLaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConnected",
                table: "Admins",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConnected",
                table: "Admins");
        }
    }
}
