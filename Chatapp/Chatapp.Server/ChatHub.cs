using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Chatapp.Client.Models;
using Chatapp.Server.Entities;
using Chatapp.Server.EntitiesData;
using Chatapp.Shared.Entities;
using Microsoft.AspNet.SignalR;

namespace Chatapp.Server
{
    public class ChatHub : Hub<IClient>
    {
        private static ConcurrentDictionary<string, User> ChatClients = new ConcurrentDictionary<string, User>();
        private static UserData userData = new UserData();
        private static FriendshipRequestData friendshipRequestData = new FriendshipRequestData();
        private static FriendshipData friendshipData = new FriendshipData();
        private static MessageData messageData = new MessageData();

        public override Task OnDisconnected(bool stopCalled)
        {
            string userName = ChatClients.SingleOrDefault((c) => c.Value.ID == Context.ConnectionId).Key;

            if (userName != null)
            {
                Clients.Others.ParticipantDisconnection(userName);
            }

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            string userName = ChatClients.SingleOrDefault((c) => c.Value.ID == Context.ConnectionId).Key;

            if (userName != null)
            {
                Clients.Others.ParticipantReconnection(userName);
            }

            return base.OnReconnected();
        }

        public LoginData Login(string username, string password)
        {
            List<User> allUsers = userData.Read(new StringFilter());
            List<User> us = userData.Read(new StringFilter() { ColumnName = "Username", Operator = "LIKE", Value = username });
            User u = us.Count == 0 ? null : us[0];

            if (u == null)
            {
                return new LoginData(null, null, "There is no such user!");
            }
            else if (u.Password != password)
            {
                return new LoginData(null, null, "Wrong password!");
            }
            else if (ChatClients.ContainsKey(username))
            {
                return new LoginData(null, null, "The profile have open\nsession already!");
            }

            u.ID = Context.ConnectionId;
            List<Friendship> friendships = friendshipData.Read(new IntFilter() { ColumnName = "User1Id", Operator = "=", Value = u.Id });
            friendships.AddRange(friendshipData.Read(new IntFilter() { ColumnName = "User2Id", Operator = "=", Value = u.Id }));
            List<User> usersOnline = new List<User>(ChatClients.Values);
            List<User> friends = new List<User>();

            for (int i = 0; i < friendships.Count; i++)
            {
                if (friendships[i].User1Id == u.Id)
                {
                    friends.Add(allUsers.FirstOrDefault((f) => f.Id == friendships[i].User2Id));
                }
                else
                {
                    friends.Add(allUsers.FirstOrDefault((f) => f.Id == friendships[i].User1Id));
                }
            }

            for (int i = 0; i < friends.Count; i++)
            {
                for (int j = 0; j < usersOnline.Count; j++)
                {
                    if (friends[i].Id == usersOnline[j].Id)
                    {
                        friends[i].IsLoggedIn = true;
                    }
                }
            }

            var added = ChatClients.TryAdd(username, u);
            if (!added) return null;
            Clients.CallerState.UserName = username;
            Clients.Others.ParticipantLogin(u);

            List<Friend> friendsForLoginData = new List<Friend>();

            friends.ForEach(f =>
            {
                List<Message> msgsFromMe = messageData.Read(new IntFilter() { ColumnName = "SenderId", Operator = "=", Value = u.Id });
                List<Message> msgsToMe = messageData.Read(new IntFilter() { ColumnName = "ReceiverId", Operator = "=", Value = u.Id });
                List<Message> msgsFromMeByFriend = msgsFromMe.Where(m => m.ReceiverId == f.Id).ToList();
                List<Message> msgsToMeByFriend = msgsToMe.Where(m => m.SenderId == f.Id).ToList();
                List<Message> allMessages = msgsFromMeByFriend;
                allMessages.AddRange(msgsToMeByFriend);
                allMessages = allMessages.OrderBy((m) => m.Id).ToList();

                ObservableCollection<ChatMessage> chatMessages = new ObservableCollection<ChatMessage>();
                allMessages.ForEach(am => chatMessages.Add(new ChatMessage
                {
                    Author = am.SenderId == u.Id ? u.Username : f.Username,
                    Time = am.Date,
                    PictureData = am.Image,
                    Message = am.Text,
                    IsOriginNative = am.SenderId == u.Id ? true : false
                }));

                friendsForLoginData.Add(new Friend() { FriendInfo = f, Messages = chatMessages });
            });

            return new LoginData(friendsForLoginData, u);
        }

        public LoginData Register(User user)
        {
            List<User> allUsers = userData.Read(new StringFilter() { });

            if (allUsers.FirstOrDefault(u => u.Username == user.Username) != null)
            {
                return new LoginData(null, null, "Already exists user\nwith the same name!");
            }

            try
            {
                userData.Create(user);

                return Login(user.Username, user.Password);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<User> Search(string sender, string filter)
        {
            User s = userData.Read(new StringFilter() { ColumnName = "Username", Operator = "LIKE", Value = sender })[0];
            List<Friendship> sFriendships = friendshipData.Read(new IntFilter() { ColumnName = "User1Id", Operator = "=", Value = s.Id });
            sFriendships.AddRange(friendshipData.Read(new IntFilter() { ColumnName = "User2Id", Operator = "=", Value = s.Id }));
            List<FriendshipRequest> sRequests = friendshipRequestData.Read(new IntFilter() { ColumnName = "SenderId", Operator = "=", Value = s.Id });
            sRequests.AddRange(friendshipRequestData.Read(new IntFilter() { ColumnName = "ReceiverId", Operator = "=", Value = s.Id }));
            List<User> allUsers = userData.Read(new StringFilter() { ColumnName = "Username", Operator = "LIKE", Value = '%' + filter + '%' });

            for (int i = 0; i < allUsers.Count; i++)
            {
                Friendship fs = sFriendships.FirstOrDefault((f) => f.User1Id == allUsers[i].Id || f.User2Id == allUsers[i].Id);
                FriendshipRequest frs = sRequests.FirstOrDefault((f) => f.ReceiverId == allUsers[i].Id || f.SenderId == allUsers[i].Id);

                if (fs != null || frs != null)
                {
                    allUsers.RemoveAt(i);
                    i--;
                }
            }

            return allUsers;
        }

        public void SendRequest(string sender, string receiver)
        {
            User s = userData.Read(new StringFilter() { ColumnName = "Username", Operator = "LIKE", Value = sender })[0];
            User r = userData.Read(new StringFilter() { ColumnName = "Username", Operator = "LIKE", Value = receiver })[0];

            friendshipRequestData.Create(new FriendshipRequest() { SenderId = s.Id, ReceiverId = r.Id });

            if (ChatClients.TryGetValue(r.Username, out User client))
            {
                Clients.Client(client.ID).UnicastFriendshipRequest(s);
            }
        }

        public void Logout()
        {
            var name = Clients.CallerState.UserName;
            if (!string.IsNullOrEmpty(name))
            {
                User client = new User();
                ChatClients.TryRemove(name, out client);
                Clients.Others.ParticipantLogout(name);
            }
        }

        public void FriendshipRequestAnswer(User sender, bool isAccepted)
        {
            var name = Clients.CallerState.UserName;
            if (!string.IsNullOrEmpty(name))
            {
                User receiver = userData.Read(new StringFilter() { ColumnName = "Username", Operator = "LIKE", Value = name })[0];
                List<FriendshipRequest> frs = friendshipRequestData.Read(new IntFilter() { ColumnName = "SenderId", Operator = "=", Value = sender.Id });
                FriendshipRequest fr = frs.FirstOrDefault((f) => f.ReceiverId == receiver.Id);

                if (fr != null)
                {
                    friendshipRequestData.Delete(fr);

                    if (isAccepted)
                    {
                        friendshipData.Create(new Friendship() { User1Id = sender.Id, User2Id = receiver.Id });

                        if (ChatClients.TryGetValue(receiver.Username, out User client))
                        {
                            sender.IsLoggedIn = true;
                            Clients.Client(client.ID).AddFriend(sender);
                        }

                        if (ChatClients.TryGetValue(sender.Username, out User client2))
                        {
                            receiver.IsLoggedIn = true;
                            Clients.Client(client2.ID).AddFriend(receiver);
                        }
                    }
                }
            }
        }

        public void BroadcastTextMessage(string message)
        {
            string name = Clients.CallerState.UserName;

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(message))
            {
                Clients.Others.BroadcastTextMessage(name, message);
            }
        }

        public void BroadcastImageMessage(byte[] img)
        {
            string name = Clients.CallerState.UserName;

            if (img != null)
            {
                Clients.Others.BroadcastPictureMessage(name, img);
            }
        }

        public void UnicastTextMessage(string recepient, string message)
        {
            string sender = Clients.CallerState.UserName;

            if (!string.IsNullOrEmpty(sender) && recepient != sender && !string.IsNullOrEmpty(message))
            {

                if (ChatClients.TryGetValue(recepient, out User client))
                {
                    Clients.Client(client.ID).UnicastTextMessage(sender, message);
                }

                int receiverId = userData.Read(new StringFilter() { ColumnName = "Username", Operator = "LIKE", Value = recepient })[0].Id;
                int senderId = userData.Read(new StringFilter() { ColumnName = "Username", Operator = "LIKE", Value = sender })[0].Id;
                messageData.Create(new Message() { SenderId = senderId, ReceiverId = receiverId, Date = DateTime.Now, Text = message, Image = null });
            }
        }

        public void UnicastFriendshipRequest(string recepient, User user)
        {
            if (recepient != user.Username && ChatClients.ContainsKey(recepient))
            {
                if (ChatClients.TryGetValue(recepient, out User client))
                {
                    Clients.Client(client.ID).UnicastFriendshipRequest(user);
                }
            }
        }

        public void UnicastImageMessage(string recepient, byte[] img)
        {
            var sender = Clients.CallerState.UserName;
            if (!string.IsNullOrEmpty(sender) && recepient != sender && img != null)
            {
                if (ChatClients.TryGetValue(recepient, out User client))
                {
                    Clients.Client(client.ID).UnicastPictureMessage(sender, img);
                }

                int receiverId = userData.Read(new StringFilter() { ColumnName = "Username", Operator = "LIKE", Value = recepient })[0].Id;
                int senderId = userData.Read(new StringFilter() { ColumnName = "Username", Operator = "LIKE", Value = sender })[0].Id;
                messageData.Create(new Message() { SenderId = senderId, ReceiverId = receiverId, Date = DateTime.Now, Text = null, Image = img });
            }
        }

        public void Typing(string recepient)
        {
            if (string.IsNullOrEmpty(recepient)) return;
            var sender = Clients.CallerState.UserName;
            User client = new User();
            ChatClients.TryGetValue(recepient, out client);
            Clients.Client(client.ID).ParticipantTyping(sender);
        }
    }
}