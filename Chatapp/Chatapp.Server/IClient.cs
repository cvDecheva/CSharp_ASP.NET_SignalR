using Chatapp.Shared.Entities;

namespace Chatapp.Server
{
    public interface IClient
    {
        void ParticipantDisconnection(string name);
        void ParticipantReconnection(string name);
        void ParticipantLogin(User client);
        void ParticipantLogout(string name);
        void BroadcastTextMessage(string sender, string message);
        void BroadcastPictureMessage(string sender, byte[] img);
        void UnicastTextMessage(string sender, string message);
        void UnicastPictureMessage(string sender, byte[] img);
        void UnicastFriendshipRequest(User user);
        void AddFriend(User user);
        void ParticipantTyping(string sender);
    }
}