using ChatApp.Shared.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Shared.Abstract
{
    public interface IServerChatHub
    {
        Task<string> GetUserIdentifier();

        Task Join(User user);
        Task JoinNotify(User sender, User receiver);
        Task SendMessageToAll(User sender, Message message);

        Task SendMessageToUser(User sender, IReadOnlyList<User> receivers, Message message);
    }
}