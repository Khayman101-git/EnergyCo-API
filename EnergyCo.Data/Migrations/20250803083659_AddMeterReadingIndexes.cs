using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnergyCo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMeterReadingIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MeterReadings_AccountId_ReadingDateTime",
                table: "MeterReadings",
                columns: new[] { "AccountId", "ReadingDateTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MeterReadings_AccountId_ReadingDateTime",
                table: "MeterReadings");
        }
    }
}
