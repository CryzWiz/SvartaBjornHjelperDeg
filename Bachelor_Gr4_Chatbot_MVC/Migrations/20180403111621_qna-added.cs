using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Bachelor_Gr4_Chatbot_MVC.Migrations
{
    public partial class qnaadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QnABaseClass",
                columns: table => new
                {
                    QnAId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    askQuestionUrl = table.Column<string>(nullable: true),
                    chatbotName = table.Column<string>(nullable: true),
                    isActive = table.Column<bool>(nullable: false),
                    knowledgeBaseID = table.Column<string>(nullable: true),
                    lastEdit = table.Column<DateTime>(nullable: false),
                    publishKnowledgeBaseUrl = table.Column<string>(nullable: true),
                    regDate = table.Column<DateTime>(nullable: false),
                    requestUrl = table.Column<string>(nullable: true),
                    subscriptionKey = table.Column<string>(nullable: true),
                    trainknowledgeBaseUrl = table.Column<string>(nullable: true),
                    updateKnowledgeBaseUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QnABaseClass", x => x.QnAId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QnABaseClass");
        }
    }
}
