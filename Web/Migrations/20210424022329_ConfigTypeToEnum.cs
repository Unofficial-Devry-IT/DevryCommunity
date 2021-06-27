using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Migrations
{
    public partial class ConfigTypeToEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ConfigType",
                table: "Configs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ConfigType",
                table: "Configs",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
