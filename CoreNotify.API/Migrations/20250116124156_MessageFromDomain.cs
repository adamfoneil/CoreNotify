using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreNotify.API.Migrations
{
    /// <inheritdoc />
    public partial class MessageFromDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SentMessage_Accounts_AccountId",
                table: "SentMessage");

            migrationBuilder.AddColumn<string>(
                name: "FromDomain",
                table: "SentMessage",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_SentMessage_Accounts_AccountId",
                table: "SentMessage",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SentMessage_Accounts_AccountId",
                table: "SentMessage");

            migrationBuilder.DropColumn(
                name: "FromDomain",
                table: "SentMessage");

            migrationBuilder.AddForeignKey(
                name: "FK_SentMessage_Accounts_AccountId",
                table: "SentMessage",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
