﻿// <auto-generated />
using Bachelor_Gr4_Chatbot_MVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Bachelor_Gr4_Chatbot_MVC.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Bachelor_Gr4_Chatbot_MVC.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<bool>("IsActive");

                    b.Property<string>("LastName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Bachelor_Gr4_Chatbot_MVC.Models.ChatbotDetails", b =>
                {
                    b.Property<int>("chatbotId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BotSecret");

                    b.Property<int>("TypeId");

                    b.Property<string>("TypeName");

                    b.Property<string>("baseUrl");

                    b.Property<string>("botAutorizeTokenScheme");

                    b.Property<string>("chatbotName");

                    b.Property<string>("contentType");

                    b.Property<string>("conversationUrlExtension");

                    b.Property<string>("conversationUrlExtensionEnding");

                    b.Property<bool>("isActive");

                    b.Property<DateTime>("lastEdit");

                    b.Property<DateTime>("regDate");

                    b.Property<string>("tokenUrlExtension");

                    b.HasKey("chatbotId");

                    b.ToTable("ChatbotDetails");
                });

            modelBuilder.Entity("Bachelor_Gr4_Chatbot_MVC.Models.ChatbotTypes", b =>
                {
                    b.Property<int>("chatBotTypeId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ChatbotDetailschatbotId");

                    b.Property<string>("Type");

                    b.Property<int>("TypeId");

                    b.HasKey("chatBotTypeId");

                    b.HasIndex("ChatbotDetailschatbotId");

                    b.ToTable("ChatbotTypes");
                });

            modelBuilder.Entity("Bachelor_Gr4_Chatbot_MVC.Models.ChatGroup", b =>
                {
                    b.Property<string>("ChatGroupName")
                        .ValueGeneratedOnAdd();

                    b.HasKey("ChatGroupName");

                    b.ToTable("ChatGroup");
                });

            modelBuilder.Entity("Bachelor_Gr4_Chatbot_MVC.Models.Conversation", b =>
                {
                    b.Property<int>("ConversationId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConversationToken");

                    b.Property<DateTime>("EndTime");

                    b.Property<bool>("IsChatBot");

                    b.Property<bool>("Result");

                    b.Property<DateTime>("StartTime");

                    b.Property<string>("UserGroup1");

                    b.Property<string>("UserGroup2");

                    b.HasKey("ConversationId");

                    b.ToTable("Conversation");
                });

            modelBuilder.Entity("Bachelor_Gr4_Chatbot_MVC.Models.Message", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<int>("ConversationId");

                    b.Property<DateTime>("DateTime");

                    b.Property<string>("DisplayName");

                    b.Property<string>("From");

                    b.Property<bool>("IsChatBot");

                    b.Property<bool>("IsChatWorker");

                    b.Property<string>("To");

                    b.HasKey("MessageId");

                    b.HasIndex("ConversationId");

                    b.ToTable("Message");
                });

            modelBuilder.Entity("Bachelor_Gr4_Chatbot_MVC.Models.OpeningHours", b =>
                {
                    b.Property<int>("OpeningHoursId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DayOfWeek");

                    b.Property<DateTime>("OpenFrom");

                    b.Property<DateTime>("OpenTo");

                    b.Property<bool>("StandardOpeningHours");

                    b.HasKey("OpeningHoursId");

                    b.ToTable("OpeningHours");
                });

            modelBuilder.Entity("Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels.QnABaseClass", b =>
                {
                    b.Property<int>("QnAId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("chatbotName");

                    b.Property<bool>("isActive");

                    b.Property<string>("knowledgeBaseID");

                    b.Property<DateTime>("lastEdit");

                    b.Property<DateTime>("regDate");

                    b.Property<string>("subscriptionKey");

                    b.HasKey("QnAId");

                    b.ToTable("QnABaseClass");
                });

            modelBuilder.Entity("Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels.QnAKnowledgeBase", b =>
                {
                    b.Property<int>("QnAKnowledgeBaseId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsActive");

                    b.Property<string>("KnowledgeBaseID");

                    b.Property<DateTime>("LastEdit");

                    b.Property<int>("QnABotId");

                    b.Property<string>("QnAKnowledgeName");

                    b.Property<DateTime>("RegDate");

                    b.HasKey("QnAKnowledgeBaseId");

                    b.ToTable("QnAKnowledgeBase");
                });

            modelBuilder.Entity("Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels.QnAPairs", b =>
                {
                    b.Property<int>("QnAPairsId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Answer");

                    b.Property<string>("KnowledgeBaseId");

                    b.Property<bool>("Published");

                    b.Property<DateTime>("PublishedDate");

                    b.Property<string>("Query");

                    b.Property<bool>("Trained");

                    b.Property<DateTime>("TrainedDate");

                    b.HasKey("QnAPairsId");

                    b.ToTable("QnAPairs");
                });

            modelBuilder.Entity("Bachelor_Gr4_Chatbot_MVC.Models.UserChatGroup", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("ChatGroupId");

                    b.Property<string>("ApplicationUserId");

                    b.HasKey("UserId", "ChatGroupId");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("ChatGroupId");

                    b.ToTable("UserChatGroup");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Bachelor_Gr4_Chatbot_MVC.Models.ChatbotTypes", b =>
                {
                    b.HasOne("Bachelor_Gr4_Chatbot_MVC.Models.ChatbotDetails")
                        .WithMany("chatbotTypes")
                        .HasForeignKey("ChatbotDetailschatbotId");
                });

            modelBuilder.Entity("Bachelor_Gr4_Chatbot_MVC.Models.Message", b =>
                {
                    b.HasOne("Bachelor_Gr4_Chatbot_MVC.Models.Conversation", "Conversation")
                        .WithMany("Messages")
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Bachelor_Gr4_Chatbot_MVC.Models.UserChatGroup", b =>
                {
                    b.HasOne("Bachelor_Gr4_Chatbot_MVC.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("ChatGroups")
                        .HasForeignKey("ApplicationUserId");

                    b.HasOne("Bachelor_Gr4_Chatbot_MVC.Models.ChatGroup", "ChatGroup")
                        .WithMany("GroupMembers")
                        .HasForeignKey("ChatGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Bachelor_Gr4_Chatbot_MVC.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Bachelor_Gr4_Chatbot_MVC.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Bachelor_Gr4_Chatbot_MVC.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Bachelor_Gr4_Chatbot_MVC.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
