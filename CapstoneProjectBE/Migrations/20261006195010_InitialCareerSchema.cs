using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CapstoneProjectBE.Migrations
{
    /// <inheritdoc />
    public partial class InitialCareerSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Careers",
                columns: table => new
                {
                    CareerID = table.Column<Guid>(type: "uuid", nullable: false),
                    CareerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Industry = table.Column<string>(type: "text", nullable: false),
                    EducationRequirements = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Careers", x => x.CareerID);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    SkillID = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.SkillID);
                });

            migrationBuilder.CreateTable(
                name: "CareerSkills",
                columns: table => new
                {
                    CareerSkillID = table.Column<Guid>(type: "uuid", nullable: false),
                    CareerID = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillID = table.Column<Guid>(type: "uuid", nullable: false),
                    ImportanceLevel = table.Column<int>(type: "integer", nullable: false),
                    IsCoreSkill = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CareerSkills", x => x.CareerSkillID);
                    table.ForeignKey(
                        name: "FK_CareerSkills_Careers_CareerID",
                        column: x => x.CareerID,
                        principalTable: "Careers",
                        principalColumn: "CareerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CareerSkills_Skills_SkillID",
                        column: x => x.SkillID,
                        principalTable: "Skills",
                        principalColumn: "SkillID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Careers",
                columns: new[] { "CareerID", "CareerName", "Description", "EducationRequirements", "Industry" },
                values: new object[] { new Guid("10000000-0000-0000-0000-000000000001"), "Software Developer", "Designs, develops and maintains software applications.", "Relevant qualification or equivalent practical experience.", "Information Technology" });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "SkillID", "Category", "Description", "SkillName" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), "Technical", "Programming using the C# language.", "C#" },
                    { new Guid("20000000-0000-0000-0000-000000000002"), "Technical", "Querying and managing relational databases.", "SQL" },
                    { new Guid("20000000-0000-0000-0000-000000000003"), "Technical", "Designing and consuming RESTful APIs.", "REST APIs" }
                });

            migrationBuilder.InsertData(
                table: "CareerSkills",
                columns: new[] { "CareerSkillID", "CareerID", "ImportanceLevel", "IsCoreSkill", "SkillID" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000001"), 5, true, new Guid("20000000-0000-0000-0000-000000000001") },
                    { new Guid("30000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000001"), 4, true, new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("30000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000001"), 4, true, new Guid("20000000-0000-0000-0000-000000000003") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Careers_CareerName",
                table: "Careers",
                column: "CareerName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CareerSkills_CareerID_SkillID",
                table: "CareerSkills",
                columns: new[] { "CareerID", "SkillID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CareerSkills_SkillID",
                table: "CareerSkills",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_SkillName",
                table: "Skills",
                column: "SkillName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CareerSkills");

            migrationBuilder.DropTable(
                name: "Careers");

            migrationBuilder.DropTable(
                name: "Skills");
        }
    }
}
