using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Bachelor_Gr4_Chatbot_MVC.Migrations
{
    public partial class Keywords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QnAKeywordPairs",
                columns: table => new
                {
                    QnAKeywordPairId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Answer = table.Column<string>(nullable: true),
                    Query = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QnAKeywordPairs", x => x.QnAKeywordPairId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QnAKeywordPairs");

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
