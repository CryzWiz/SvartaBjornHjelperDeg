using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Bachelor_Gr4_Chatbot_MVC.Migrations
{
    public partial class chatbot3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "converationUrlExtenison",
                table: "ChatbotDetails",
                newName: "conversationUrlExtenison");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "conversationUrlExtenison",
                table: "ChatbotDetails",
                newName: "converationUrlExtenison");
        }
    }
}
