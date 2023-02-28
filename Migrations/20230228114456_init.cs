using Microsoft.EntityFrameworkCore.Migrations;

namespace Lol.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DBYoloV4s",
                columns: table => new
                {
                    ImageID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(nullable: true),
                    BBox0 = table.Column<float>(nullable: false),
                    BBox1 = table.Column<float>(nullable: false),
                    BBox2 = table.Column<float>(nullable: false),
                    BBox3 = table.Column<float>(nullable: false),
                    Label = table.Column<string>(nullable: true),
                    Confidence = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBYoloV4s", x => x.ImageID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DBYoloV4s");
        }
    }
}
