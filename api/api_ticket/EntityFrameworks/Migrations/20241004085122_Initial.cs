using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_ticket.EntityFrameworks.Migrations
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

            migrationBuilder.CreateTable(
                name: "ms_user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Fullname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PhoneNo = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
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
                    table.PrimaryKey("PK_ms_user", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ms_ticket",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_ms_ticket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ms_ticket_ms_event_EventId",
                        column: x => x.EventId,
                        principalTable: "ms_event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ms_ticket_EventId",
                table: "ms_ticket",
                column: "EventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ms_ticket");

            migrationBuilder.DropTable(
                name: "ms_user");

            migrationBuilder.DropTable(
                name: "ms_event");
        }
    }
}
