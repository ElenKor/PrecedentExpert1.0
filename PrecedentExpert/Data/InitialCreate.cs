using Microsoft.EntityFrameworkCore.Migrations;

namespace PrecedentExpert.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "objects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_objects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "precedents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolutionParams = table.Column<int[]>(nullable: true),
                    SituationParams = table.Column<int[]>(nullable: true),
                    ObjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_precedents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_precedents_objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "situation_variables",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    ObjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_situation_variables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_situation_variables_objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "solution_variables",
                columns: table => new
                {
                    SolutionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    ObjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_solution_variables", x => x.SolutionId);
                    table.ForeignKey(
                        name: "FK_solution_variables_objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_precedents_ObjectId",
                table: "precedents",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_situation_variables_ObjectId",
                table: "situation_variables",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_solution_variables_ObjectId",
                table: "solution_variables",
                column: "ObjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "precedents");

            migrationBuilder.DropTable(
                name: "situation_variables");

            migrationBuilder.DropTable(
                name: "solution_variables");

            migrationBuilder.DropTable(
                name: "objects");
        }
    }
}
