using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreNotify.API.Migrations
{
    /// <inheritdoc />
    public partial class ContinuationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SerilogContinuationMarker_Accounts_AccountId",
                table: "SerilogContinuationMarker");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SerilogContinuationMarker",
                table: "SerilogContinuationMarker");

            migrationBuilder.RenameTable(
                name: "SerilogContinuationMarker",
                newName: "ContinuationMarkers");

            migrationBuilder.RenameIndex(
                name: "IX_SerilogContinuationMarker_AccountId_Name",
                table: "ContinuationMarkers",
                newName: "IX_ContinuationMarkers_AccountId_Name");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ContinuationMarkers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContinuationMarkers",
                table: "ContinuationMarkers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContinuationMarkers_Accounts_AccountId",
                table: "ContinuationMarkers",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContinuationMarkers_Accounts_AccountId",
                table: "ContinuationMarkers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContinuationMarkers",
                table: "ContinuationMarkers");

            migrationBuilder.RenameTable(
                name: "ContinuationMarkers",
                newName: "SerilogContinuationMarker");

            migrationBuilder.RenameIndex(
                name: "IX_ContinuationMarkers_AccountId_Name",
                table: "SerilogContinuationMarker",
                newName: "IX_SerilogContinuationMarker_AccountId_Name");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SerilogContinuationMarker",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SerilogContinuationMarker",
                table: "SerilogContinuationMarker",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SerilogContinuationMarker_Accounts_AccountId",
                table: "SerilogContinuationMarker",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
