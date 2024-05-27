using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Account_Management.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("0fd927a3-216d-4c3a-82a1-02d079508d3d"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("167876d8-c588-4279-9c28-7f34fd9b1b6f"));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("00b0da86-599d-4ab8-9bca-a07ab0b08ce9"), "Reader" },
                    { new Guid("728c33fe-b40e-4b93-9f86-b1226c7a8a04"), "Writer" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("00b0da86-599d-4ab8-9bca-a07ab0b08ce9"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("728c33fe-b40e-4b93-9f86-b1226c7a8a04"));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("0fd927a3-216d-4c3a-82a1-02d079508d3d"), "Reader" },
                    { new Guid("167876d8-c588-4279-9c28-7f34fd9b1b6f"), "Writer" }
                });
        }
    }
}
