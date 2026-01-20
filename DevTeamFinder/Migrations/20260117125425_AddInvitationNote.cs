using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevTeamFinder.Migrations
{
    /// <inheritdoc />
    public partial class AddInvitationNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Not",
                table: "Invitations",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Not",
                table: "Invitations");
        }
    }
}
