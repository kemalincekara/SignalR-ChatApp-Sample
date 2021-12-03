using ChatApp.Shared.Abstract;
using ChatApp.Shared.Entities;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Client.ChatHub
{
    public class ChatHubConnection : IChatHubConnection
    {
        #region Event Handler

        public event EventHandler<MessageHandlerResult> MessageHandler;

        public event EventHandler<User> UserJoinedHandler;

        public event EventHandler<User> UserLeftHandler;

        #endregion Event Handler

        #region Properties

        public List<User> Users { get; private set; }
        public HubConnectionState State => _hubConnection.State;

        #endregion Properties

        #region Private Fields

        private readonly HubConnection _hubConnection;
        private readonly User _user;

        #endregion Private Fields

        public ChatHubConnection(User user)
        {
            Users = new List<User>();
            _user = user;
            _hubConnection = new HubConnectionBuilder()
                     .WithUrl($"https://localhost:5001/chathub?userId={user.Id}")
                     .AddMessagePackProtocol()
                     .WithAutomaticReconnect()
                     .Build(); ;
            _hubConnection.On<User, Message>(nameof(IClientChatHub.MessageHandler), (user, message) => MessageHandler?.Invoke(this, new MessageHandlerResult(user, message)));
            _hubConnection.On<User>(nameof(IClientChatHub.UserJoinedHandler), user =>
            {
                if (!Users.Any(p => p.Id == user.Id))
                {
                    Users.Add(user);
                    JoinNotify(_user, user);
                    UserJoinedHandler?.Invoke(this, user);
                }
            });
            _hubConnection.On<string>(nameof(IClientChatHub.UserLeftHandler), userId =>
            {
                var find = Users.FirstOrDefault(i => i.Id == userId);
                if (find != null)
                {
                    Users.Remove(find);
                    UserLeftHandler?.Invoke(this, find);
                }
            });
        }

        public Task StartAsync(CancellationToken cancellationToken = default) => _hubConnection.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken = default) => _hubConnection.StopAsync(cancellationToken);

        #region IServerChatHub
        public Task<string> GetUserIdentifier() => _hubConnection.InvokeAsync<string>(nameof(IServerChatHub.GetUserIdentifier));

        public Task Join(User user) => _hubConnection.InvokeAsync(nameof(IServerChatHub.Join), user);

        public Task JoinNotify(User sender, User receiver) => _hubConnection.InvokeAsync(nameof(IServerChatHub.JoinNotify), sender, receiver);

        public Task SendMessageToAll(User user, Message message) => _hubConnection.InvokeAsync(nameof(IServerChatHub.SendMessageToAll), user, message);

        public Task SendMessageToUser(User sender, IReadOnlyList<User> receivers, Message message) => _hubConnection.InvokeAsync(nameof(IServerChatHub.SendMessageToUser), sender, receivers, message);

        #endregion
    }
}