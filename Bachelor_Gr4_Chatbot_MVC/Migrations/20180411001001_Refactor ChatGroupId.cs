using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Bachelor_Gr4_Chatbot_MVC.Migrations
{
    public partial class RefactorChatGroupId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChatGroupName",
                table: "ChatGroup",
                newName: "ChatGroupId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ChatGroup",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ChatGroup");

            migrationBuilder.RenameColumn(
                name: "ChatGroupId",
                table: "ChatGroup",
                newName: "ChatGroupName");
        }
    }
}
