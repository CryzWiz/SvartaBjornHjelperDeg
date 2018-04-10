using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Bachelor_Gr4_Chatbot_MVC.Migrations
{
    public partial class QnA_update1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QnAKnowledgeBaseId",
                table: "QnAPairs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QnAPairs_QnAKnowledgeBaseId",
                table: "QnAPairs",
                column: "QnAKnowledgeBaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_QnAPairs_QnAKnowledgeBase_QnAKnowledgeBaseId",
                table: "QnAPairs",
                column: "QnAKnowledgeBaseId",
                principalTable: "QnAKnowledgeBase",
                principalColumn: "QnAKnowledgeBaseId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QnAPairs_QnAKnowledgeBase_QnAKnowledgeBaseId",
                table: "QnAPairs");

            migrationBuilder.DropIndex(
                name: "IX_QnAPairs_QnAKnowledgeBaseId",
                table: "QnAPairs");

            migrationBuilder.DropColumn(
                name: "QnAKnowledgeBaseId",
                table: "QnAPairs");
        }
    }
}
