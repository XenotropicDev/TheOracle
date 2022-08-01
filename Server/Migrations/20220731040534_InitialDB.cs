using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Server.Migrations
{
    public partial class InitialDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CharacterAssets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatorDiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    AssetId = table.Column<string>(type: "text", nullable: false),
                    SelectedAbilities = table.Column<string>(type: "text", nullable: false),
                    Inputs = table.Column<string>(type: "text", nullable: false),
                    ConditionValue = table.Column<int>(type: "integer", nullable: false),
                    ThumbnailURL = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterAssets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerCharacters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    DiscordGuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    MessageId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    ChannelId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Edge = table.Column<int>(type: "integer", nullable: false),
                    Heart = table.Column<int>(type: "integer", nullable: false),
                    Iron = table.Column<int>(type: "integer", nullable: false),
                    Shadow = table.Column<int>(type: "integer", nullable: false),
                    Wits = table.Column<int>(type: "integer", nullable: false),
                    Health = table.Column<int>(type: "integer", nullable: false),
                    Spirit = table.Column<int>(type: "integer", nullable: false),
                    Supply = table.Column<int>(type: "integer", nullable: false),
                    Momentum = table.Column<int>(type: "integer", nullable: false),
                    XpGained = table.Column<int>(type: "integer", nullable: false),
                    XpSpent = table.Column<int>(type: "integer", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: true),
                    Impacts = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCharacters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    DiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Game = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.DiscordId);
                });

            migrationBuilder.CreateTable(
                name: "ProgressTrackers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: false),
                    Ticks = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressTrackers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCharacters_MessageId",
                table: "PlayerCharacters",
                column: "MessageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterAssets");

            migrationBuilder.DropTable(
                name: "PlayerCharacters");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "ProgressTrackers");
        }
    }
}
