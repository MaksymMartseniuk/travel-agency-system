using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace travel_agency_system.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserTypeAndRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_PayerId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "PaymentTransactionTravelPackage");

            migrationBuilder.RenameColumn(
                name: "Discriminator",
                table: "Users",
                newName: "UserType");

            migrationBuilder.AlterColumn<long>(
                name: "Duration",
                table: "TravelPackages",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time(6)");

            migrationBuilder.AlterColumn<string>(
                name: "Activities",
                table: "TravelPackages",
                type: "varchar(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TransactionTours",
                columns: table => new
                {
                    PaymentTransactionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TravelPackageId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTours", x => new { x.PaymentTransactionId, x.TravelPackageId });
                    table.ForeignKey(
                        name: "FK_TransactionTours_Transactions_PaymentTransactionId",
                        column: x => x.PaymentTransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionTours_TravelPackages_TravelPackageId",
                        column: x => x.TravelPackageId,
                        principalTable: "TravelPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTours_TravelPackageId",
                table: "TransactionTours",
                column: "TravelPackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_PayerId",
                table: "Transactions",
                column: "PayerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_PayerId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "TransactionTours");

            migrationBuilder.RenameColumn(
                name: "UserType",
                table: "Users",
                newName: "Discriminator");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Duration",
                table: "TravelPackages",
                type: "time(6)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Activities",
                table: "TravelPackages",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_PayerId",
                table: "Transactions",
                column: "PayerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
