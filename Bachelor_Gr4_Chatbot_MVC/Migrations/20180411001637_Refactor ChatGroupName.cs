using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Bachelor_Gr4_Chatbot_MVC.Migrations
{
    public partial class RefactorChatGroupName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ChatGroup",
                newName: "ChatGroupName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChatGroupName",
                table: "ChatGroup",
                newName: "Name");
        }
    }
}
