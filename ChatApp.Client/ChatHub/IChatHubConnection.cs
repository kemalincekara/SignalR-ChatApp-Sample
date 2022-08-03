using ChatApp.Shared.Abstract;
using ChatApp.Shared.Entities;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChatApp.Client.ChatHub
{
    public interface IChatHubConnection : IServerChatHub
    {
        HubConnectionState State { get; }
        List<User> Users { get; }

        event EventHandler<MessageHandlerResult> MessageHandler;

        event EventHandler<User> UserJoinedHandler;

        event EventHandler<User> UserLeftHandler;

        Task StartAsync(CancellationToken cancellationToken = default);

        Task StopAsync(CancellationToken cancellationToken = default);
    }
}