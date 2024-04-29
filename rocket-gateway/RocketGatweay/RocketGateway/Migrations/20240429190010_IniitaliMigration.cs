using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RocketGateway.Migrations
{
    /// <inheritdoc />
    public partial class IniitaliMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rockets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    LaunchSpeed = table.Column<int>(type: "integer", nullable: false),
                    Mission = table.Column<string>(type: "text", nullable: false),
                    ExplodedReason = table.Column<string>(type: "text", nullable: false),
                    WasExploded = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rockets", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rockets");
        }
    }
}
