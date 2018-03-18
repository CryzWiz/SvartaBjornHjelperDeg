using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.Repositories
{
    public interface IChatRepository
    {
        Task SaveOpeningHours(OpeningHours openingHours);
        Task SaveOpeningHours(IEnumerable<OpeningHours> openingHours);

    }
}
