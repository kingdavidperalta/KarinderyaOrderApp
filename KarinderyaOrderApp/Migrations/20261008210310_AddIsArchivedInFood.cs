using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KarinderyaOrderApp.Migrations
{
    /// <inheritdoc />
    public partial class AddIsArchivedInFood : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Foods",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Foods");
        }
    }
}
