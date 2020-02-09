using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryWebsite.Migrations
{
    public partial class DefaultUserRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "6e29740f-407f-4f3e-b4a4-744344d0f2a7", "bda3cb1e-6ecc-4906-8f86-edd1faa96a4a", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "41ef98ad-2338-4cd8-b154-dcbaf21ce424", "70e220a7-6899-4b98-8153-f8ba8766a542", "Librarian", "LIBRARIAN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7e636afb-8e97-4619-a0d6-bf564442f09a", "d7b2db1a-158d-4565-91f9-6231319787ce", "Reader", "READER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "41ef98ad-2338-4cd8-b154-dcbaf21ce424");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6e29740f-407f-4f3e-b4a4-744344d0f2a7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7e636afb-8e97-4619-a0d6-bf564442f09a");
        }
    }
}
