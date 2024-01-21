using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pdfsharp_online_backend.Migrations
{
    public partial class AddedAttachmentCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Base64AttachmentCode",
                table: "EditViews",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Base64AttachmentCode",
                table: "EditViews");
        }
    }
}
