using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bull_chat_backend.Migrations
{
    /// <inheritdoc />
    public partial class addindexondatetime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Message_Date",
                table: "Message",
                column: "Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Message_Date",
                table: "Message");
        }
    }
}
