using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace travel_agency_system.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTransactionsManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TravelPackages_Transactions_PaymentTransactionId",
                table: "TravelPackages");

            migrationBuilder.DropIndex(
                name: "IX_TravelPackages_PaymentTransactionId",
                table: "TravelPackages");

            migrationBuilder.DropColumn(
                name: "PaymentTransactionId",
                table: "TravelPackages");

            migrationBuilder.CreateTable(
                name: "PaymentTransactionTravelPackage",
                columns: table => new
                {
                    PaymentTransactionsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PurchasedToursId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactionTravelPackage", x => new { x.PaymentTransactionsId, x.PurchasedToursId });
                    table.ForeignKey(
                        name: "FK_PaymentTransactionTravelPackage_Transactions_PaymentTransact~",
                        column: x => x.PaymentTransactionsId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentTransactionTravelPackage_TravelPackages_PurchasedTour~",
                        column: x => x.PurchasedToursId,
                        principalTable: "TravelPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactionTravelPackage_PurchasedToursId",
                table: "PaymentTransactionTravelPackage",
                column: "PurchasedToursId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentTransactionTravelPackage");

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentTransactionId",
                table: "TravelPackages",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_TravelPackages_PaymentTransactionId",
                table: "TravelPackages",
                column: "PaymentTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TravelPackages_Transactions_PaymentTransactionId",
                table: "TravelPackages",
                column: "PaymentTransactionId",
                principalTable: "Transactions",
                principalColumn: "Id");
        }
    }
}
