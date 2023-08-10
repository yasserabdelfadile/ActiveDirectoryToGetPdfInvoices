using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication25.Migrations
{
    /// <inheritdoc />
    public partial class CreatTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Site",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Site_ID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Site", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserID",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SamAccountName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserID", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EntityRoleSiteEntity",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    SiteID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityRoleSiteEntity", x => new { x.RoleID, x.SiteID });
                    table.ForeignKey(
                        name: "FK_EntityRoleSiteEntity_Role_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Role",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityRoleSiteEntity_Site_SiteID",
                        column: x => x.SiteID,
                        principalTable: "Site",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleSite",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    SiteID = table.Column<int>(type: "int", nullable: false),
                    SiteEntityID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleSite", x => new { x.RoleID, x.SiteID });
                    table.ForeignKey(
                        name: "FK_RoleSite_Role_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Role",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleSite_Site_SiteEntityID",
                        column: x => x.SiteEntityID,
                        principalTable: "Site",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_RoleSite_Site_SiteID",
                        column: x => x.SiteID,
                        principalTable: "Site",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityRoleEntityUserID",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    UserIDID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityRoleEntityUserID", x => new { x.RoleID, x.UserIDID });
                    table.ForeignKey(
                        name: "FK_EntityRoleEntityUserID_Role_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Role",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityRoleEntityUserID_UserID_UserIDID",
                        column: x => x.UserIDID,
                        principalTable: "UserID",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleUserID",
                columns: table => new
                {
                    EntityRoleID = table.Column<int>(type: "int", nullable: false),
                    EntityUser = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUserID", x => new { x.EntityRoleID, x.EntityUser });
                    table.ForeignKey(
                        name: "FK_RoleUserID_Role_EntityRoleID",
                        column: x => x.EntityRoleID,
                        principalTable: "Role",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleUserID_UserID_EntityUser",
                        column: x => x.EntityUser,
                        principalTable: "UserID",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityRoleEntityUserID_UserIDID",
                table: "EntityRoleEntityUserID",
                column: "UserIDID");

            migrationBuilder.CreateIndex(
                name: "IX_EntityRoleSiteEntity_SiteID",
                table: "EntityRoleSiteEntity",
                column: "SiteID");

            migrationBuilder.CreateIndex(
                name: "IX_RoleSite_SiteEntityID",
                table: "RoleSite",
                column: "SiteEntityID");

            migrationBuilder.CreateIndex(
                name: "IX_RoleSite_SiteID",
                table: "RoleSite",
                column: "SiteID");

            migrationBuilder.CreateIndex(
                name: "IX_RoleUserID_EntityUser",
                table: "RoleUserID",
                column: "EntityUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityRoleEntityUserID");

            migrationBuilder.DropTable(
                name: "EntityRoleSiteEntity");

            migrationBuilder.DropTable(
                name: "RoleSite");

            migrationBuilder.DropTable(
                name: "RoleUserID");

            migrationBuilder.DropTable(
                name: "Site");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "UserID");
        }
    }
}
