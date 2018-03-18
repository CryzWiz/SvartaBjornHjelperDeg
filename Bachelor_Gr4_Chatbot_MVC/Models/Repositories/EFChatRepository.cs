using Bachelor_Gr4_Chatbot_MVC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.Repositories
{
    public class EFChatRepository : IChatRepository
    {
        private ApplicationDbContext _db;
        
        public EFChatRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task SaveOpeningHours(OpeningHours openingHours)
        {
            await _db.OpeningHours.AddAsync(openingHours);
        }

        public async Task SaveOpeningHours(IEnumerable<OpeningHours> openingHours)
        {
            await _db.OpeningHours.AddRangeAsync(openingHours);
        }
    }
}
