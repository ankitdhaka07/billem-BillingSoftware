using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanySettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanySettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    PhoneNumbers = table.Column<string>(type: "TEXT", nullable: false),
                    GstNo = table.Column<string>(type: "TEXT", nullable: false),
                    PANNo = table.Column<string>(type: "TEXT", nullable: false),
                    TINNo = table.Column<string>(type: "TEXT", nullable: false),
                    BankName = table.Column<string>(type: "TEXT", nullable: false),
                    AccountNumber = table.Column<string>(type: "TEXT", nullable: false),
                    IFSCCode = table.Column<string>(type: "TEXT", nullable: false),
                    BranchLocation = table.Column<string>(type: "TEXT", nullable: false),
                    LogoPath = table.Column<string>(type: "TEXT", nullable: false),
                    CgstPercentage = table.Column<double>(type: "REAL", nullable: false),
                    SgstPercentage = table.Column<double>(type: "REAL", nullable: false),
                    TermsAndConditions = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanySettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanySettings");
        }
    }
}
