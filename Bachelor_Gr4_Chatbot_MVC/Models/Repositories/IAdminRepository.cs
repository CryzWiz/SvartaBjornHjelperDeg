using Bachelor_Gr4_Chatbot_MVC.Models.AdministratorViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.Repositories
{
    public interface IAdminRepository
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetUser(String username);
        Task<bool> DeactivateUser(String username);
        Task<bool> ActivateUser(String username);
        Task<bool> UpdateUserData(User user);
        Task<bool> AddChatGroup(ChatGroup chatGroup);
        Task<IEnumerable<ChatGroupViewModel>> GetAllChatGroupsVM();
        Task<ChatGroup> GetChatGroupByIdAsync(string id);
        Task<bool> UpdateChatGroupAsync(ChatGroup chatGroup);
        Task<bool> DeleteChatGroupAsync(string id);
    }
}
