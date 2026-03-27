using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillItems_Items_ItemId1",
                table: "BillItems");

            migrationBuilder.DropIndex(
                name: "IX_BillItems_ItemId1",
                table: "BillItems");

            migrationBuilder.DropColumn(
                name: "ItemId1",
                table: "BillItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "BillItems",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "BillItems",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemId1",
                table: "BillItems",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BillItems_ItemId1",
                table: "BillItems",
                column: "ItemId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BillItems_Items_ItemId1",
                table: "BillItems",
                column: "ItemId1",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
