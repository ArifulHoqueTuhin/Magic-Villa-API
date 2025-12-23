using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVillaApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocalUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Roles = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VillaList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Details = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    SqFt = table.Column<int>(type: "int", nullable: false),
                    Occupancy = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    Amenity = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VillaList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Villa Number",
                columns: table => new
                {
                    Villa_No = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Villa_Id = table.Column<int>(type: "int", nullable: false),
                    Villa_Details = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Villa Number", x => x.Villa_No);
                    table.ForeignKey(
                        name: "FK_Villa Number_VillaList",
                        column: x => x.Villa_Id,
                        principalTable: "VillaList",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VillaNumberv2",
                columns: table => new
                {
                    VillaNo = table.Column<int>(type: "int", nullable: false),
                    VillaId = table.Column<int>(type: "int", nullable: false),
                    SpecialDetails = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VillaNumber", x => new { x.VillaNo, x.VillaId });
                    table.ForeignKey(
                        name: "FK_VillaNumber_VillaList",
                        column: x => x.VillaId,
                        principalTable: "VillaList",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Villa Number_Villa_Id",
                table: "Villa Number",
                column: "Villa_Id");

            migrationBuilder.CreateIndex(
                name: "IX_VillaNumberv2_VillaId",
                table: "VillaNumberv2",
                column: "VillaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocalUsers");

            migrationBuilder.DropTable(
                name: "Villa Number");

            migrationBuilder.DropTable(
                name: "VillaNumberv2");

            migrationBuilder.DropTable(
                name: "VillaList");
        }
    }
}
