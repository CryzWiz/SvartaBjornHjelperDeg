using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Bachelor_Gr4_Chatbot_MVC.Migrations
{
    public partial class Linkedconversation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConversationToken",
                table: "Conversation");

            migrationBuilder.AddColumn<int>(
                name: "LinkedConversation",
                table: "Conversation",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkedConversation",
                table: "Conversation");

            migrationBuilder.AddColumn<string>(
                name: "ConversationToken",
                table: "Conversation",
                nullable: true);
        }
    }
}
