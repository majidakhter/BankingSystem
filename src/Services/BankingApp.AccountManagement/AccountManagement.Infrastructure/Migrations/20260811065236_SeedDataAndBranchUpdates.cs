using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataAndBranchUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Bank");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Bank");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Bank");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Bank");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Bank");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "Bank");

            migrationBuilder.AddColumn<string>(
                name: "IfscCode",
                table: "Branches",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MICRCode",
                table: "Branches",
                type: "integer",
                maxLength: 20,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FundTransferTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    DestinationAccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    TransferType = table.Column<int>(type: "integer", nullable: false),
                    PaymentGateway = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    BeneficiaryAccountNo = table.Column<string>(type: "text", nullable: true),
                    IfscCode = table.Column<string>(type: "text", nullable: true),
                    UpiId = table.Column<string>(type: "text", nullable: true),
                    DestinationBankName = table.Column<string>(type: "text", nullable: true),
                    GatewayTransactionRef = table.Column<string>(type: "text", nullable: true),
                    FailureReason = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundTransferTransactions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FundTransferTransactions");

            migrationBuilder.DropColumn(
                name: "IfscCode",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "MICRCode",
                table: "Branches");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Bank",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Bank",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Bank",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Bank",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Bank",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "Bank",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
