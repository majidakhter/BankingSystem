using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboxTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationBankName",
                table: "FundTransferTransactions");

            migrationBuilder.DropColumn(
                name: "UpiId",
                table: "FundTransferTransactions");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "FundTransferTransactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TransferToEntity",
                table: "FundTransferTransactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BeneficaryIfscCode",
                table: "Beneficaries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "outbox",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    aggregate_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    aggregate_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    event_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_outbox_created",
                table: "outbox",
                column: "created_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outbox");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "FundTransferTransactions");

            migrationBuilder.DropColumn(
                name: "TransferToEntity",
                table: "FundTransferTransactions");

            migrationBuilder.DropColumn(
                name: "BeneficaryIfscCode",
                table: "Beneficaries");

            migrationBuilder.AddColumn<string>(
                name: "DestinationBankName",
                table: "FundTransferTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpiId",
                table: "FundTransferTransactions",
                type: "text",
                nullable: true);
        }
    }
}
