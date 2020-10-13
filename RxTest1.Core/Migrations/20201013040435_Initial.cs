using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RxTest.Core.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NewsSources",
                columns: table => new
                {
                    SourceUrl = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    PublicationDate = table.Column<DateTime>(type: "date", nullable: false),
                    ImageUrl = table.Column<string>(nullable: true),
                    Author = table.Column<string>(nullable: true),
                    HomeUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsSources", x => x.SourceUrl);
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Url = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    SourceUrl = table.Column<string>(nullable: true),
                    PublicationDate = table.Column<DateTime>(type: "date", nullable: false),
                    LastActiveDate = table.Column<DateTime>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true),
                    Author = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Url);
                    table.ForeignKey(
                        name: "FK_Articles_NewsSources_SourceUrl",
                        column: x => x.SourceUrl,
                        principalTable: "NewsSources",
                        principalColumn: "SourceUrl",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_SourceUrl",
                table: "Articles",
                column: "SourceUrl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "NewsSources");
        }
    }
}
