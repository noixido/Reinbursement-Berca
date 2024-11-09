using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class updatemodelaccountdetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id_Account = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role_Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id_Account);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id_Category = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Category_Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id_Category);
                });

            migrationBuilder.CreateTable(
                name: "Titles",
                columns: table => new
                {
                    Id_Title = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reimburse_Limit = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Titles", x => x.Id_Title);
                });

            migrationBuilder.CreateTable(
                name: "Reimbursements",
                columns: table => new
                {
                    Id_Reimbursement = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id_Category = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Evidence = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<float>(type: "real", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Submit_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reimbursements", x => x.Id_Reimbursement);
                    table.ForeignKey(
                        name: "FK_Reimbursements_Categories_Id_Category",
                        column: x => x.Id_Category,
                        principalTable: "Categories",
                        principalColumn: "Id_Category");
                });

            migrationBuilder.CreateTable(
                name: "AccountDetails",
                columns: table => new
                {
                    Id_AccountDetail = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id_Title = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Birth_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Join_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Current_Limit = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountDetails", x => x.Id_AccountDetail);
                    table.ForeignKey(
                        name: "FK_AccountDetails_Accounts_Id_AccountDetail",
                        column: x => x.Id_AccountDetail,
                        principalTable: "Accounts",
                        principalColumn: "Id_Account",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountDetails_Titles_Id_Title",
                        column: x => x.Id_Title,
                        principalTable: "Titles",
                        principalColumn: "Id_Title",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReimbursementProfilings",
                columns: table => new
                {
                    Id_Account = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id_Reimbursement = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReimbursementProfilings", x => new { x.Id_Account, x.Id_Reimbursement });
                    table.ForeignKey(
                        name: "FK_ReimbursementProfilings_Accounts_Id_Account",
                        column: x => x.Id_Account,
                        principalTable: "Accounts",
                        principalColumn: "Id_Account",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReimbursementProfilings_Reimbursements_Id_Reimbursement",
                        column: x => x.Id_Reimbursement,
                        principalTable: "Reimbursements",
                        principalColumn: "Id_Reimbursement",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountDetails_Id_Title",
                table: "AccountDetails",
                column: "Id_Title");

            migrationBuilder.CreateIndex(
                name: "IX_ReimbursementProfilings_Id_Reimbursement",
                table: "ReimbursementProfilings",
                column: "Id_Reimbursement");

            migrationBuilder.CreateIndex(
                name: "IX_Reimbursements_Id_Category",
                table: "Reimbursements",
                column: "Id_Category");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountDetails");

            migrationBuilder.DropTable(
                name: "ReimbursementProfilings");

            migrationBuilder.DropTable(
                name: "Titles");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Reimbursements");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
