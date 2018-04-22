using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Bachelor_Gr4_Chatbot_MVC.Migrations
{
    public partial class updateconversationClass1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KnowledgebaseId",
                table: "Conversation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QnABaseId",
                table: "Conversation",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KnowledgebaseId",
                table: "Conversation");

            migrationBuilder.DropColumn(
                name: "QnABaseId",
                table: "Conversation");
        }
    }
}
