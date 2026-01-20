using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevTeamFinder.Migrations
{
    /// <inheritdoc />
    public partial class AddInvitationSender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SQLite foreign key değişikliklerini desteklemediği için tabloyu yeniden oluşturuyoruz
            
            // 1. Yeni tablo oluştur
            migrationBuilder.CreateTable(
                name: "Invitations_New",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    DeveloperId = table.Column<int>(type: "INTEGER", nullable: false),
                    SenderId = table.Column<int>(type: "INTEGER", nullable: false),
                    Durum = table.Column<string>(type: "TEXT", nullable: false),
                    Not = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invitations_Developers_DeveloperId",
                        column: x => x.DeveloperId,
                        principalTable: "Developers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invitations_Developers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Developers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invitations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // 2. Eski verileri kopyala (SenderId'yi proje sahibi olarak ayarla)
            migrationBuilder.Sql(@"
                INSERT INTO Invitations_New (Id, ProjectId, DeveloperId, SenderId, Durum, ""Not"")
                SELECT 
                    i.Id, 
                    i.ProjectId, 
                    i.DeveloperId, 
                    p.DeveloperId as SenderId,
                    i.Durum, 
                    i.""Not""
                FROM Invitations i
                INNER JOIN Projects p ON p.Id = i.ProjectId
            ");

            // 3. Eski tabloyu sil
            migrationBuilder.DropTable(name: "Invitations");

            // 4. Yeni tabloyu yeniden adlandır
            migrationBuilder.RenameTable(
                name: "Invitations_New",
                newName: "Invitations");

            // 5. Index'leri oluştur
            migrationBuilder.CreateIndex(
                name: "IX_Invitations_DeveloperId",
                table: "Invitations",
                column: "DeveloperId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_SenderId",
                table: "Invitations",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_ProjectId_DeveloperId",
                table: "Invitations",
                columns: new[] { "ProjectId", "DeveloperId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Geri alma işlemi için eski tabloyu yeniden oluştur
            migrationBuilder.CreateTable(
                name: "Invitations_Old",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    DeveloperId = table.Column<int>(type: "INTEGER", nullable: false),
                    Durum = table.Column<string>(type: "TEXT", nullable: false),
                    Not = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invitations_Developers_DeveloperId",
                        column: x => x.DeveloperId,
                        principalTable: "Developers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invitations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(@"
                INSERT INTO Invitations_Old (Id, ProjectId, DeveloperId, Durum, ""Not"")
                SELECT Id, ProjectId, DeveloperId, Durum, ""Not""
                FROM Invitations
            ");

            migrationBuilder.DropTable(name: "Invitations");

            migrationBuilder.RenameTable(
                name: "Invitations_Old",
                newName: "Invitations");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_DeveloperId",
                table: "Invitations",
                column: "DeveloperId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_ProjectId_DeveloperId",
                table: "Invitations",
                columns: new[] { "ProjectId", "DeveloperId" },
                unique: true);
        }
    }
}
