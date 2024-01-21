using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pdfsharp_online_backend.Migrations
{
    public partial class RenameImageNameToImageData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageName",
                table: "ImageItems",
                newName: "ImageData");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageData",
                table: "ImageItems",
                newName: "ImageName");
        }
    }
}
