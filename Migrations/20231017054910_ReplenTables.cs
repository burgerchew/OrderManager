using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderManagerEF.Migrations
{
    /// <inheritdoc />
    public partial class ReplenTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
         
            migrationBuilder.CreateTable(
                name: "ReplenHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReplenDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TradingRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarehouseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplenHeaders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReplenishmentResults",
                columns: table => new
                {
                    CustomerCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BulkBin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RetailBin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BulkQty = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RetailQty = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AccountingRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LocationNo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

         

            migrationBuilder.CreateTable(
                name: "ReplenDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReplenId = table.Column<int>(type: "int", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    FromLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToLocation = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplenDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReplenDetails_ReplenHeaders_ReplenId",
                        column: x => x.ReplenId,
                        principalTable: "ReplenHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReplenDetails_ReplenId",
                table: "ReplenDetails",
                column: "ReplenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
   

            migrationBuilder.DropTable(
                name: "ReplenDetails");

            migrationBuilder.DropTable(
                name: "ReplenHeaders");
        }
    }
}
