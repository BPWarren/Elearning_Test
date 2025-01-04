using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elearning_Test.Migrations
{
    /// <inheritdoc />
    public partial class AdminPrenomColl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Prenom",
                table: "Admins",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Prenom",
                table: "Admins");
        }
    }
}
