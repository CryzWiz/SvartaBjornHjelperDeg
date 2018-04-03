using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Bachelor_Gr4_Chatbot_MVC.Migrations
{
    public partial class qnaupdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QnAKnowledgeBase",
                columns: table => new
                {
                    QnAKnowledgeBaseId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    KnowledgeBaseID = table.Column<string>(nullable: true),
                    LastEdit = table.Column<DateTime>(nullable: false),
                    QnABotId = table.Column<int>(nullable: false),
                    QnAKnowledgeName = table.Column<string>(nullable: true),
                    RegDate = table.Column<DateTime>(nullable: false),
                    RequestUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QnAKnowledgeBase", x => x.QnAKnowledgeBaseId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QnAKnowledgeBase");
        }
    }
}
