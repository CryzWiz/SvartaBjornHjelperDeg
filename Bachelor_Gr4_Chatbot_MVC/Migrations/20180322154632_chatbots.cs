using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Bachelor_Gr4_Chatbot_MVC.Migrations
{
    public partial class chatbots : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatbotDetails",
                columns: table => new
                {
                    chatbotId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BotSecret = table.Column<string>(nullable: true),
                    base_Url = table.Column<string>(nullable: true),
                    botAutorizeTokenScheme = table.Column<string>(nullable: true),
                    chatbotName = table.Column<string>(nullable: true),
                    contentType = table.Column<string>(nullable: true),
                    converationUrlExtenison = table.Column<string>(nullable: true),
                    tokenUrlExtension = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatbotDetails", x => x.chatbotId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatbotDetails");
        }
    }
}
