using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bull_chat_backend.Migrations
{
    /// <inheritdoc />
    public partial class restruct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Message_ContentId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "Content");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "User",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "ContentType",
                table: "Content",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Item",
                table: "Content",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Message_ContentId",
                table: "Message",
                column: "ContentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Message_ContentId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "Item",
                table: "Content");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "User",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Content",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Message_ContentId",
                table: "Message",
                column: "ContentId",
                unique: true);
        }
    }
}
