namespace Chatapp.Shared.Entities
{
    public class FriendshipRequest : Entity
    {
        public int SenderId { get; set; }

        public int ReceiverId { get; set; }
    }
}
