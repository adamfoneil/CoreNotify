using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreNotify.API.Migrations
{
    /// <inheritdoc />
    public partial class Webhooks2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Webhook_Accounts_AccountId",
                table: "Webhook");

            migrationBuilder.DropForeignKey(
                name: "FK_WebhookLog_Webhook_WebhookId",
                table: "WebhookLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WebhookLog",
                table: "WebhookLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Webhook",
                table: "Webhook");

            migrationBuilder.RenameTable(
                name: "WebhookLog",
                newName: "WebhookLogs");

            migrationBuilder.RenameTable(
                name: "Webhook",
                newName: "Webhooks");

            migrationBuilder.RenameIndex(
                name: "IX_WebhookLog_WebhookId",
                table: "WebhookLogs",
                newName: "IX_WebhookLogs_WebhookId");

            migrationBuilder.RenameIndex(
                name: "IX_Webhook_AccountId_Name",
                table: "Webhooks",
                newName: "IX_Webhooks_AccountId_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WebhookLogs",
                table: "WebhookLogs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Webhooks",
                table: "Webhooks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WebhookLogs_Webhooks_WebhookId",
                table: "WebhookLogs",
                column: "WebhookId",
                principalTable: "Webhooks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Webhooks_Accounts_AccountId",
                table: "Webhooks",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WebhookLogs_Webhooks_WebhookId",
                table: "WebhookLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Webhooks_Accounts_AccountId",
                table: "Webhooks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Webhooks",
                table: "Webhooks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WebhookLogs",
                table: "WebhookLogs");

            migrationBuilder.RenameTable(
                name: "Webhooks",
                newName: "Webhook");

            migrationBuilder.RenameTable(
                name: "WebhookLogs",
                newName: "WebhookLog");

            migrationBuilder.RenameIndex(
                name: "IX_Webhooks_AccountId_Name",
                table: "Webhook",
                newName: "IX_Webhook_AccountId_Name");

            migrationBuilder.RenameIndex(
                name: "IX_WebhookLogs_WebhookId",
                table: "WebhookLog",
                newName: "IX_WebhookLog_WebhookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Webhook",
                table: "Webhook",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WebhookLog",
                table: "WebhookLog",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Webhook_Accounts_AccountId",
                table: "Webhook",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WebhookLog_Webhook_WebhookId",
                table: "WebhookLog",
                column: "WebhookId",
                principalTable: "Webhook",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
