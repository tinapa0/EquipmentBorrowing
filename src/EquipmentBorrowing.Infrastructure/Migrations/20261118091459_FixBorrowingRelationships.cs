using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquipmentBorrowing.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixBorrowingRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Borrowings_Equipment_EquipmentId1",
                table: "Borrowings");

            migrationBuilder.DropForeignKey(
                name: "FK_Borrowings_Students_StudentId1",
                table: "Borrowings");

            migrationBuilder.DropIndex(
                name: "IX_Borrowings_EquipmentId1",
                table: "Borrowings");

            migrationBuilder.DropIndex(
                name: "IX_Borrowings_StudentId1",
                table: "Borrowings");

            migrationBuilder.DropColumn(
                name: "EquipmentId1",
                table: "Borrowings");

            migrationBuilder.DropColumn(
                name: "StudentId1",
                table: "Borrowings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EquipmentId1",
                table: "Borrowings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentId1",
                table: "Borrowings",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Borrowings_EquipmentId1",
                table: "Borrowings",
                column: "EquipmentId1");

            migrationBuilder.CreateIndex(
                name: "IX_Borrowings_StudentId1",
                table: "Borrowings",
                column: "StudentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Borrowings_Equipment_EquipmentId1",
                table: "Borrowings",
                column: "EquipmentId1",
                principalTable: "Equipment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Borrowings_Students_StudentId1",
                table: "Borrowings",
                column: "StudentId1",
                principalTable: "Students",
                principalColumn: "Id");
        }
    }
}
