using Chatapp.Client.Enums;
using Chatapp.Client.Models;
using Chatapp.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chatapp.Client.Services
{
    public interface IChatService
    {
        event Action<User> ParticipantLoggedIn;
        event Action<string> ParticipantLoggedOut;
        event Action<string> ParticipantDisconnected;
        event Action<string> ParticipantReconnected;
        event Action ConnectionReconnecting;
        event Action ConnectionReconnected;
        event Action ConnectionClosed;
        event Action<string, string, MessageType> NewTextMessage;
        event Action<string, byte[], MessageType> NewImageMessage;
        event Action<User, MessageType> NewFriendshipRequest;
        event Action<User, MessageType> NewFriendshipAdd;
        event Action<string> ParticipantTyping;

        Task ConnectAsync();
        Task<LoginData> LoginAsync(string name, string password);
        Task<LoginData> RegisterAsync(User user);
        Task<List<User>> SearchAsync(string sender, string filter);
        Task SendRequestAsync(string sender, string receiver);
        Task LogoutAsync();
        Task FriendshipRequestAnswerAsync(User sender, bool isAccepted);
        Task SendBroadcastMessageAsync(string msg);
        Task SendBroadcastMessageAsync(byte[] img);
        Task SendUnicastMessageAsync(string recepient, string msg);
        Task SendUnicastMessageAsync(string recepient, byte[] img);
        Task TypingAsync(string recepient);
    }
}
