using ChatApp.Shared.Entities;

namespace ChatApp.Shared.Abstract
{
    public interface IClientChatHub
    {
        Task MessageHandler(User sender, Message message);

        Task UserJoinedHandler(User user);

        Task UserLeftHandler(string id);
    }
}