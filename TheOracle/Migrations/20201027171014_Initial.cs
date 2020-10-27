using Microsoft.EntityFrameworkCore.Migrations;

namespace TheOracle.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChannelSettings",
                columns: table => new
                {
                    ChannelID = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DefaultGame = table.Column<int>(nullable: false),
                    Culture = table.Column<string>(nullable: true),
                    RerollDuplicates = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelSettings", x => x.ChannelID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelSettings");
        }
    }
}
