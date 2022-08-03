using ChatApp.Shared.Abstract;
using ChatApp.Shared.Entities;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Server.Hubs
{
    public class ChatHub : Hub<IClientChatHub>, IServerChatHub
    {
        public override Task OnConnectedAsync()
        {
            System.Console.WriteLine($"{Context.UserIdentifier} connected");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(System.Exception exception)
        {
            System.Console.WriteLine($"{Context.UserIdentifier} disconnected");
            Clients.Others.UserLeftHandler(Context.UserIdentifier);
            return base.OnDisconnectedAsync(exception);
        }

        public Task<string> GetUserIdentifier() => Task.FromResult(Context.UserIdentifier);

        public async Task Join(User user) => await Clients.Others.UserJoinedHandler(user);

        public async Task JoinNotify(User sender, User receiver) => await Clients.User(receiver.Id).UserJoinedHandler(sender);

        public async Task SendMessageToAll(User sender, Message message) => await Clients.All.MessageHandler(sender, message);

        public async Task SendMessageToUser(User sender, IReadOnlyList<User> receivers, Message message)
        {
            await Clients.User(sender.Id).MessageHandler(sender, message);
            await Clients.Users(receivers.Select(i => i.Id)).MessageHandler(sender, message);
        }
    }
}