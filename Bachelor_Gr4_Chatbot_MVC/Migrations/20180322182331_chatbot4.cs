using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Bachelor_Gr4_Chatbot_MVC.Migrations
{
    public partial class chatbot4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "conversationUrlExtenison",
                table: "ChatbotDetails",
                newName: "conversationUrlExtension");

            migrationBuilder.RenameColumn(
                name: "base_Url",
                table: "ChatbotDetails",
                newName: "baseUrl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "conversationUrlExtension",
                table: "ChatbotDetails",
                newName: "conversationUrlExtenison");

            migrationBuilder.RenameColumn(
                name: "baseUrl",
                table: "ChatbotDetails",
                newName: "base_Url");
        }
    }
}
