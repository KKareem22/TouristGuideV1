using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouristGuide.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditedForSomeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GuideProfileId",
                table: "Trips",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "Tourists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PreferredLanguage",
                table: "Tourists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ExperienceYears",
                table: "GuideProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Languages",
                table: "GuideProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_GuideProfileId",
                table: "Trips",
                column: "GuideProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_GuideProfiles_GuideProfileId",
                table: "Trips",
                column: "GuideProfileId",
                principalTable: "GuideProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_GuideProfiles_GuideProfileId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_GuideProfileId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "GuideProfileId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "Tourists");

            migrationBuilder.DropColumn(
                name: "PreferredLanguage",
                table: "Tourists");

            migrationBuilder.DropColumn(
                name: "ExperienceYears",
                table: "GuideProfiles");

            migrationBuilder.DropColumn(
                name: "Languages",
                table: "GuideProfiles");
        }
    }
}
