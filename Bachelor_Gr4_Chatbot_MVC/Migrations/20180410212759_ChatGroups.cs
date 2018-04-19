using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Bachelor_Gr4_Chatbot_MVC.Migrations
{
    public partial class ChatGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatGroup",
                columns: table => new
                {
                    ChatGroupName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatGroup", x => x.ChatGroupName);
                });

            migrationBuilder.CreateTable(
                name: "UserChatGroup",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    ChatGroupId = table.Column<string>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChatGroup", x => new { x.UserId, x.ChatGroupId });
                    table.ForeignKey(
                        name: "FK_UserChatGroup_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserChatGroup_ChatGroup_ChatGroupId",
                        column: x => x.ChatGroupId,
                        principalTable: "ChatGroup",
                        principalColumn: "ChatGroupName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserChatGroup_ApplicationUserId",
                table: "UserChatGroup",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChatGroup_ChatGroupId",
                table: "UserChatGroup",
                column: "ChatGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserChatGroup");

            migrationBuilder.DropTable(
                name: "ChatGroup");
        }
    }
}
