﻿using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Bachelor_Gr4_Chatbot_MVC.Migrations
{
    public partial class QnA_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestUrl",
                table: "QnAKnowledgeBase");

            migrationBuilder.DropColumn(
                name: "askQuestionUrl",
                table: "QnABaseClass");

            migrationBuilder.DropColumn(
                name: "publishKnowledgeBaseUrl",
                table: "QnABaseClass");

            migrationBuilder.DropColumn(
                name: "requestUrl",
                table: "QnABaseClass");

            migrationBuilder.DropColumn(
                name: "trainknowledgeBaseUrl",
                table: "QnABaseClass");

            migrationBuilder.DropColumn(
                name: "updateKnowledgeBaseUrl",
                table: "QnABaseClass");

            migrationBuilder.CreateTable(
                name: "QnAPairs",
                columns: table => new
                {
                    QnAPairsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Answer = table.Column<string>(nullable: true),
                    KnowledgeBaseId = table.Column<string>(nullable: true),
                    Published = table.Column<bool>(nullable: false),
                    PublishedDate = table.Column<DateTime>(nullable: false),
                    Query = table.Column<string>(nullable: true),
                    Trained = table.Column<bool>(nullable: false),
                    TrainedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QnAPairs", x => x.QnAPairsId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QnAPairs");

            migrationBuilder.AddColumn<string>(
                name: "RequestUrl",
                table: "QnAKnowledgeBase",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "askQuestionUrl",
                table: "QnABaseClass",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "publishKnowledgeBaseUrl",
                table: "QnABaseClass",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "requestUrl",
                table: "QnABaseClass",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "trainknowledgeBaseUrl",
                table: "QnABaseClass",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updateKnowledgeBaseUrl",
                table: "QnABaseClass",
                nullable: true);
        }
    }
}
