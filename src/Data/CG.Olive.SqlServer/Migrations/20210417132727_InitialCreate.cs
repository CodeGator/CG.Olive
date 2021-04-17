using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CG.Olive.SqlServer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Olive");

            migrationBuilder.CreateTable(
                name: "Applications",
                schema: "Olive",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Sid = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2021, 4, 17, 8, 27, 26, 899, DateTimeKind.Local).AddTicks(7566)),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Environments",
                schema: "Olive",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2021, 4, 17, 8, 27, 26, 909, DateTimeKind.Local).AddTicks(9256)),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Environments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Uploads",
                schema: "Olive",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    EnvironmentId = table.Column<int>(type: "int", nullable: false),
                    Json = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2021, 4, 17, 8, 27, 26, 912, DateTimeKind.Local).AddTicks(1882)),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uploads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Uploads_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "Olive",
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Uploads_Environments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalSchema: "Olive",
                        principalTable: "Environments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                schema: "Olive",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UploadId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EnvironmentId = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsSecret = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2021, 4, 17, 8, 27, 26, 921, DateTimeKind.Local).AddTicks(8531)),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Settings_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "Olive",
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Settings_Environments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalSchema: "Olive",
                        principalTable: "Environments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Settings_Uploads_UploadId",
                        column: x => x.UploadId,
                        principalSchema: "Olive",
                        principalTable: "Uploads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Name",
                schema: "Olive",
                table: "Applications",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Environments_Name",
                schema: "Olive",
                table: "Environments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Settings_ApplicationId",
                schema: "Olive",
                table: "Settings",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_EnvironmentId",
                schema: "Olive",
                table: "Settings",
                column: "EnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_Key_EnvironmentId_ApplicationId_UploadId",
                schema: "Olive",
                table: "Settings",
                columns: new[] { "Key", "EnvironmentId", "ApplicationId", "UploadId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Settings_UploadId",
                schema: "Olive",
                table: "Settings",
                column: "UploadId");

            migrationBuilder.CreateIndex(
                name: "IX_Uploads_ApplicationId_EnvironmentId",
                schema: "Olive",
                table: "Uploads",
                columns: new[] { "ApplicationId", "EnvironmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Uploads_EnvironmentId",
                schema: "Olive",
                table: "Uploads",
                column: "EnvironmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings",
                schema: "Olive");

            migrationBuilder.DropTable(
                name: "Uploads",
                schema: "Olive");

            migrationBuilder.DropTable(
                name: "Applications",
                schema: "Olive");

            migrationBuilder.DropTable(
                name: "Environments",
                schema: "Olive");
        }
    }
}
