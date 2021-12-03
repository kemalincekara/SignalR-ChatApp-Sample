using ChatApp.Shared.Entities;

namespace ChatApp.Client.ChatHub
{
    public class MessageHandlerResult
    {
        public MessageHandlerResult(User user, Message message)
        {
            Sender = user;
            Message = message;
        }

        public User Sender { get; set; }
        public Message Message { get; set; }
    }
}