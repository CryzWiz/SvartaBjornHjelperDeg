using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Bachelor_Gr4_Chatbot_MVC.Models;
using Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels;

namespace Bachelor_Gr4_Chatbot_MVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Conversation> Conversations { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<OpeningHours> OpeningHours {get; set; }
        // Chatbotdetails so we can store several chatbots
        // Just microsoft bots atm
        public virtual DbSet<ChatbotDetails> ChatbotDetails { get; set; }
        public virtual DbSet<ChatbotTypes> ChatbotTypes { get; set; }
        // QnA bots
        public virtual DbSet<QnABaseClass> QnABaseClass { get; set; }
        public virtual DbSet<QnAKnowledgeBase> QnAKnowledgeBase { get; set; }
        public virtual DbSet<QnAPairs> QnAPairs { get; set; }
        public virtual DbSet<QnAKeywordPair> QnAKeywordPairs { get; set; }


        // Chat Groups
        public virtual DbSet<ChatGroup> ChatGroups { get; set; }
        public virtual DbSet<UserChatGroup> UserChatGroup { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<Conversation>().ToTable("Conversation");
            builder.Entity<Message>().ToTable("Message");
            builder.Entity<ChatGroup>().ToTable("ChatGroup");
            builder.Entity<UserChatGroup>()
                .HasKey(x => new { x.UserId, x.ChatGroupId });
        }
    }
}
