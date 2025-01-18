using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreNotify.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTimestamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SentMessage_Accounts_AccountId",
                table: "SentMessage");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_SentMessage_MessageId",
                table: "SentMessage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SentMessage",
                table: "SentMessage");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "SentMessage");

            migrationBuilder.RenameTable(
                name: "SentMessage",
                newName: "SentMessages");

            migrationBuilder.RenameIndex(
                name: "IX_SentMessage_AccountId",
                table: "SentMessages",
                newName: "IX_SentMessages_AccountId");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_SentMessages_MessageId",
                table: "SentMessages",
                column: "MessageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SentMessages",
                table: "SentMessages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SentMessages_Accounts_AccountId",
                table: "SentMessages",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SentMessages_Accounts_AccountId",
                table: "SentMessages");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_SentMessages_MessageId",
                table: "SentMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SentMessages",
                table: "SentMessages");

            migrationBuilder.RenameTable(
                name: "SentMessages",
                newName: "SentMessage");

            migrationBuilder.RenameIndex(
                name: "IX_SentMessages_AccountId",
                table: "SentMessage",
                newName: "IX_SentMessage_AccountId");

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "SentMessage",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_SentMessage_MessageId",
                table: "SentMessage",
                column: "MessageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SentMessage",
                table: "SentMessage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SentMessage_Accounts_AccountId",
                table: "SentMessage",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
