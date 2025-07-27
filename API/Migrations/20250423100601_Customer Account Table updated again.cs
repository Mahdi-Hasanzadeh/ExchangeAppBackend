using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class CustomerAccountTableupdatedagain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "CustomerAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "IdCardImage",
                table: "CustomerAccounts",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PassportImage",
                table: "CustomerAccounts",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RegistrationImage",
                table: "CustomerAccounts",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "CustomerAccounts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "IdCardImage",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "PassportImage",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "RegistrationImage",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "CustomerAccounts");
        }
    }
}
