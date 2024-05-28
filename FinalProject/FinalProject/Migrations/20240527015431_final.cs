using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject.Migrations
{
    public partial class final : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1716eb86-812a-45c3-bfc7-51dcef7a89d9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2958e137-3b97-404a-9af3-29eee66d77be");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "5a875ab5-adeb-4fee-a4fb-32bc23625c07", "78449557-321b-4c2a-b81d-f159ac568346", "Admin", "admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "d5bf3bdf-2b79-41c0-93d9-ded386843f59", "ce399bd4-6e6d-4073-b161-2c61dbe7cebe", "User", "user" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5a875ab5-adeb-4fee-a4fb-32bc23625c07");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d5bf3bdf-2b79-41c0-93d9-ded386843f59");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1716eb86-812a-45c3-bfc7-51dcef7a89d9", "2719dfa0-6d3f-4fda-a163-51e16c706f03", "User", "user" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "2958e137-3b97-404a-9af3-29eee66d77be", "2167dfd7-e090-4d51-8399-2348ea2483c1", "Admin", "admin" });
        }
    }
}
