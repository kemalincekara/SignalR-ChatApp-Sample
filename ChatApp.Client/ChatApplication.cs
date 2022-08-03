using ChatApp.Client.ChatHub;
using ChatApp.Shared.Entities;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChatApp.Client
{
    public class ChatApplication
    {
        private IChatHubConnection _client;
        private User _user;
        private bool _menuSelected;

        public async Task StartClientAsync()
        {
            string? name;
            do
            {
                Console.Write("Name: ");
                name = Console.ReadLine();
            } while (string.IsNullOrEmpty(name));

            _user = new User()
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = name
            };

            _client = new ChatHubConnection(_user);
            _client.MessageHandler += new EventHandler<MessageHandlerResult>(Client_MessageHandler);
            _client.UserJoinedHandler += new EventHandler<User>(Client_UserJoinedHandler);
            _client.UserLeftHandler += new EventHandler<User>(Client_UserLeftHandler);
            await _client.StartAsync();

            if (_client.State == HubConnectionState.Connected)
            {
                await _client.Join(_user);
                await ShowMenu();
                await _client.StopAsync();
            }
        }

        private async Task ShowMenu()
        {
            _menuSelected = false;
            Console.WriteLine(@"Menu:
1) Send Message to All Users
2) Send Message to a user
3) Exit");
        select:
            var select = Console.ReadKey(true);
            switch (select.KeyChar)
            {
                case '1': await SendMessageToAllUsers(); break;
                case '2': await SendMessageToUser(); break;
                case '3': return;
                default: goto select;
            }
            await ShowMenu();
        }

        private async Task SendMessageToAllUsers()
        {
            _menuSelected = true;
            Console.Write("Your Message: ");
            string? message;
            while (_client.State == HubConnectionState.Connected && !string.IsNullOrEmpty(message = ReadLineOrEsc()))
            {
                await _client.SendMessageToAll(_user, new Message(message));
            }
        }

        private async Task SendMessageToUser()
        {
            Console.WriteLine("Select a user from list:");
            for (int i = 0; i < _client.Users.Count; i++)
                Console.WriteLine("{0} - {1}", i, _client.Users[i].Name);
            Console.Write("Select: ");
            if (!int.TryParse(Console.ReadLine(), out int index) || index < 0 || index >= _client.Users.Count)
                return;

            User selected = _client.Users[index];

            _menuSelected = true;

            Console.Write("Your Message to {0}: ", selected.Name);
            string? message;
            while (_client.State == HubConnectionState.Connected && !string.IsNullOrEmpty(message = ReadLineOrEsc()))
            {
                await _client.SendMessageToUser(_user, new User[] { selected }, new Message(message));
            }
        }

        private void Client_UserJoinedHandler(object? sender, User e) => WriteLine(ConsoleColor.Green, $"{e.Name} is Online");

        private void Client_UserLeftHandler(object? sender, User e) => WriteLine(ConsoleColor.Red, $"{e.Name} is Offline");

        private void Client_MessageHandler(object? sender, MessageHandlerResult e) => WriteLine(ConsoleColor.Magenta, "{0}: {1}", e.Sender.Name, e.Message.Content);

        private static readonly object _writeLineLock = new();

        private void WriteLine(ConsoleColor color, string format, params object[] args)
        {
            lock (_writeLineLock)
            {
                if (_menuSelected)
                    ClearLine();

                if (!string.IsNullOrEmpty(format))
                {
                    Console.ForegroundColor = color;
                    Console.WriteLine(format, args);
                    Console.ResetColor();
                }
                if (_menuSelected)
                    Console.Write("Your Message: ");
            }
        }

        private static void ClearLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        // returns null if user pressed Escape, or the contents of the line if they pressed Enter.
        private static string? ReadLineOrEsc()
        {
            string retString = "";
            int curIndex = 0;
            do
            {
                ConsoleKeyInfo readKeyResult = Console.ReadKey(true);

                // handle Esc
                if (readKeyResult.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine();
                    return null;
                }

                // handle Enter
                if (readKeyResult.Key == ConsoleKey.Enter)
                {
                    ClearLine();
                    return retString;
                }

                // handle backspace
                if (readKeyResult.Key == ConsoleKey.Backspace)
                {
                    if (curIndex > 0)
                    {
                        retString = retString.Remove(retString.Length - 1);
                        Console.Write(readKeyResult.KeyChar);
                        Console.Write(' ');
                        Console.Write(readKeyResult.KeyChar);
                        curIndex--;
                    }
                }
                else
                // handle all other keypresses
                {
                    retString += readKeyResult.KeyChar;
                    Console.Write(readKeyResult.KeyChar);
                    curIndex++;
                }
            }
            while (true);
        }
    }
}