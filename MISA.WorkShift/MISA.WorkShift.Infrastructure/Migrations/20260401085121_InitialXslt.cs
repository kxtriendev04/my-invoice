using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MISA.WorkShift.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialXslt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "xslt_content",
                table: "invoice_templates",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xslt_content",
                table: "invoice_templates");
        }
    }
}
