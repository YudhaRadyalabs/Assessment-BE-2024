using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_event.EntityFrameworks.Migations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ms_event",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EventDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    Total = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByName = table.Column<string>(type: "text", nullable: false),
                    UpdatedByName = table.Column<string>(type: "text", nullable: false),
                    DeletedByName = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ms_event", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ms_event");
        }
    }
}
