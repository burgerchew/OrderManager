using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderManagerEF.Migrations
{
    /// <inheritdoc />
    public partial class UserPrinter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrinterName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrinterName",
                table: "Users");
        }
    }
}
