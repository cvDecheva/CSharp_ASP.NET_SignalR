using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Chatapp.Client.Enums;
using Chatapp.Client.Models;
using Chatapp.Shared.Entities;
using Microsoft.AspNet.SignalR.Client;

namespace Chatapp.Client.Services
{
    public class ChatService : IChatService
    {
        public event Action<string, string, MessageType> NewTextMessage;
        public event Action<string, byte[], MessageType> NewImageMessage;
        public event Action<User, MessageType> NewFriendshipRequest;
        public event Action<User, MessageType> NewFriendshipAdd;
        public event Action<string> ParticipantDisconnected;
        public event Action<User> ParticipantLoggedIn;
        public event Action<string> ParticipantLoggedOut;
        public event Action<string> ParticipantReconnected;
        public event Action ConnectionReconnecting;
        public event Action ConnectionReconnected;
        public event Action ConnectionClosed;
        public event Action<string> ParticipantTyping;

        private IHubProxy hubProxy;
        private HubConnection connection;
        private string url = "https://localhost:44318";

        public async Task ConnectAsync()
        {
            connection = new HubConnection(url);
            hubProxy = connection.CreateHubProxy("ChatHub");
            hubProxy.On<User>("ParticipantLogin", (u) => ParticipantLoggedIn?.Invoke(u));
            hubProxy.On<string>("ParticipantLogout", (n) => ParticipantLoggedOut?.Invoke(n));
            hubProxy.On<string>("ParticipantDisconnection", (n) => ParticipantDisconnected?.Invoke(n));
            hubProxy.On<string>("ParticipantReconnection", (n) => ParticipantReconnected?.Invoke(n));
            hubProxy.On<string, string>("BroadcastTextMessage", (n, m) => NewTextMessage?.Invoke(n, m, MessageType.Broadcast));
            hubProxy.On<string, byte[]>("BroadcastPictureMessage", (n, m) => NewImageMessage?.Invoke(n, m, MessageType.Broadcast));
            hubProxy.On<string, string>("UnicastTextMessage", (n, m) => NewTextMessage?.Invoke(n, m, MessageType.Unicast));
            hubProxy.On<string, byte[]>("UnicastPictureMessage", (n, m) => NewImageMessage?.Invoke(n, m, MessageType.Unicast));
            hubProxy.On<User>("UnicastFriendshipRequest", (u) => NewFriendshipRequest?.Invoke(u, MessageType.Unicast));
            hubProxy.On<User>("AddFriend", (u) => NewFriendshipAdd?.Invoke(u, MessageType.Unicast));
            hubProxy.On<string>("ParticipantTyping", (p) => ParticipantTyping?.Invoke(p));

            connection.Reconnecting += Reconnecting;
            connection.Reconnected += Reconnected;
            connection.Closed += Disconnected;

            ServicePointManager.DefaultConnectionLimit = 10;
            await connection.Start();
        }

        private void Disconnected()
        {
            ConnectionClosed?.Invoke();
        }

        private void Reconnected()
        {
            ConnectionReconnected?.Invoke();
        }

        private void Reconnecting()
        {
            ConnectionReconnecting?.Invoke();
        }

        public async Task<LoginData> LoginAsync(string name, string password)
        {
            return await hubProxy.Invoke<LoginData>("Login", new object[] { name, password});
        }

        public async Task<LoginData> RegisterAsync(User user)
        {
            return await hubProxy.Invoke<LoginData>("Register", new object[] { user });
        }

        public async Task<List<User>> SearchAsync(string sender, string filter)
        {
            return await hubProxy.Invoke<List<User>>("Search", new object[] { sender, filter });
        }

        public async Task SendRequestAsync(string sender, string receiver)
        {
            await hubProxy.Invoke("SendRequest", new object[] { sender, receiver });
        }

        public async Task LogoutAsync()
        {
            await hubProxy.Invoke("Logout");
        }

        public async Task FriendshipRequestAnswerAsync(User sender, bool isAccepted)
        {
            await hubProxy.Invoke("FriendshipRequestAnswer", new object[] { sender, isAccepted });
        }

        public async Task SendBroadcastMessageAsync(string msg)
        {
            await hubProxy.Invoke("BroadcastTextMessage", msg);
        }

        public async Task SendBroadcastMessageAsync(byte[] img)
        {
            await hubProxy.Invoke("BroadcastImageMessage", img);
        }

        public async Task SendUnicastMessageAsync(string recepient, string msg)
        {
            await hubProxy.Invoke("UnicastTextMessage", new object[] { recepient, msg });
        }

        public async Task SendUnicastMessageAsync(string recepient, byte[] img)
        {
            await hubProxy.Invoke("UnicastImageMessage", new object[] { recepient, img });
        }

        public async Task TypingAsync(string recepient)
        {
            await hubProxy.Invoke("Typing", recepient);
        }
    }
}
