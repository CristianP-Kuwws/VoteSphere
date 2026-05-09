using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVote470Plus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeCandidateIdNullableOnVotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Votes",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 6, 4, 14, 40, 577, DateTimeKind.Utc).AddTicks(8630),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 27, 1, 48, 48, 708, DateTimeKind.Utc).AddTicks(2143));

            migrationBuilder.AlterColumn<int>(
                name: "CandidateId",
                table: "Votes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Votes",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 27, 1, 48, 48, 708, DateTimeKind.Utc).AddTicks(2143),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 6, 4, 14, 40, 577, DateTimeKind.Utc).AddTicks(8630));

            migrationBuilder.AlterColumn<int>(
                name: "CandidateId",
                table: "Votes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
