using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BankingApp.LoanManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialLoanManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DebtorInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdentificationNumber = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtorInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ScoringResult = table.Column<string>(type: "text", nullable: true),
                    Score_Explanation = table.Column<string>(type: "text", nullable: false),
                    MonthlyIncomeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyValueAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Asset_Address_Street = table.Column<string>(type: "text", nullable: false),
                    Asset_Address_City = table.Column<string>(type: "text", nullable: false),
                    Asset_Address_State = table.Column<string>(type: "text", nullable: false),
                    Asset_Address_ZipCode = table.Column<string>(type: "text", nullable: false),
                    Asset_Address_Country = table.Column<string>(type: "text", nullable: false),
                    LoanAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Loan_LoanNumberOfYears = table.Column<int>(type: "integer", nullable: false),
                    InterestRate = table.Column<decimal>(type: "numeric", nullable: false),
                    Decision_DecisionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DecisionBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Registration_RegistrationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Registration_RegisteredBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Operators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompetenceLevelAmount = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Debt",
                columns: table => new
                {
                    DebtorInfoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debt", x => new { x.DebtorInfoId, x.Id });
                    table.ForeignKey(
                        name: "FK_Debt_DebtorInfos_DebtorInfoId",
                        column: x => x.DebtorInfoId,
                        principalTable: "DebtorInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "LoanTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Mortgage" },
                    { 2, "Education" },
                    { 3, "Home" },
                    { 4, "Car" },
                    { 5, "Personal" },
                    { 6, "Gold" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Debt");

            migrationBuilder.DropTable(
                name: "LoanApplications");

            migrationBuilder.DropTable(
                name: "LoanTypes");

            migrationBuilder.DropTable(
                name: "Operators");

            migrationBuilder.DropTable(
                name: "DebtorInfos");
        }
    }
}
