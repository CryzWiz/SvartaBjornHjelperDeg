using Bachelor_Gr4_Chatbot_MVC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bachelor_Gr4_Chatbot_MVC.Models.ChatViewModels;
using Microsoft.EntityFrameworkCore;

namespace Bachelor_Gr4_Chatbot_MVC.Models.Repositories
{
    public class EFChatRepository : IChatRepository
    {
        private ApplicationDbContext _db;
        
        public EFChatRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ChatOpeningHoursViewModel>> GetAllOpeningHours()
        {
            IEnumerable<ChatOpeningHoursViewModel> openingHours = 
                await(from hours in _db.OpeningHours
                      orderby hours.DayOfWeek
                                     select new ChatOpeningHoursViewModel
                                     {
                                         OpeningHoursId = hours.OpeningHoursId,
                                         WeekDay = hours.DayOfWeek,
                                         DateFrom = (hours.OpenFrom),
                                         DateTo = (hours.OpenTo),
                                         OpenFrom = (hours.OpenFrom),
                                         OpenTo = (hours.OpenTo),
                                         IsStandard = hours.StandardOpeningHours

                                     }).ToListAsync();
            return openingHours;
        }

        public async Task SaveOpeningHours(OpeningHours openingHours)
        {
            await _db.OpeningHours.AddAsync(openingHours);
            await _db.SaveChangesAsync();
        }

        public async Task SaveOpeningHours(IEnumerable<OpeningHours> openingHours)
        {
            await _db.AddRangeAsync(openingHours);
            await _db.SaveChangesAsync();
        }

        public async Task<int> AddConversationAsync(Conversation conversation)
        {
            await _db.AddAsync(conversation);
            await _db.SaveChangesAsync();
            return conversation.ConversationId;
        }

        public async Task<string> GetName(string userId)
        {
            ApplicationUser user =  await _db.Users.Where(x => x.UserName == userId).FirstOrDefaultAsync();
            return user.FirstName;
        }

        public async Task AddMessageAsync(Message message)
        {
            await _db.AddAsync(message);
            await _db.SaveChangesAsync();
        }

        public async Task AddMessagesAsync(IEnumerable<Message> messages)
        {
            await _db.Messages.AddRangeAsync(messages);
            await _db.SaveChangesAsync();
        }

        public async Task<Conversation> GetConversationByIdAsync(int conversationId)
        {
            Conversation conversation = await _db.Conversations.Where(x => x.ConversationId == conversationId).SingleOrDefaultAsync();
            return conversation;
        }

        public async Task UpdateConversationAsync(Conversation conversation)
        {
            _db.Conversations.Update(conversation);
            await _db.SaveChangesAsync();
        }
    }
}
