using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Bachelor_Gr4_Chatbot_MVC.Migrations
{
    public partial class chatbotdetailsupd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "ChatbotDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TypeName",
                table: "ChatbotDetails",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChatbotTypes",
                columns: table => new
                {
                    chatBotTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChatbotDetailschatbotId = table.Column<int>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    TypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatbotTypes", x => x.chatBotTypeId);
                    table.ForeignKey(
                        name: "FK_ChatbotTypes_ChatbotDetails_ChatbotDetailschatbotId",
                        column: x => x.ChatbotDetailschatbotId,
                        principalTable: "ChatbotDetails",
                        principalColumn: "chatbotId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatbotTypes_ChatbotDetailschatbotId",
                table: "ChatbotTypes",
                column: "ChatbotDetailschatbotId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatbotTypes");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "ChatbotDetails");

            migrationBuilder.DropColumn(
                name: "TypeName",
                table: "ChatbotDetails");
        }
    }
}
