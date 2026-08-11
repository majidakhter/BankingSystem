using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAccountNoSequenceDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"Accounts\" ALTER COLUMN \"AccountNo\" DROP DEFAULT;");

            migrationBuilder.DropSequence(
                name: "MySimpleSequence");

            migrationBuilder.AlterColumn<int>(
                name: "AccountNo",
                table: "Accounts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValueSql: "nextval('\"MySimpleSequence\"')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "MySimpleSequence",
                startValue: 1000001L,
                minValue: -2000000L,
                maxValue: 2000000L,
                cyclic: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccountNo",
                table: "Accounts",
                type: "integer",
                nullable: false,
                defaultValueSql: "nextval('\"MySimpleSequence\"')",
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
