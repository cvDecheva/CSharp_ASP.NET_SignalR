using System;

namespace Chatapp.Shared.Entities
{
    public class Message : Entity
    {
        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

        public DateTime Date { get; set; }

        public string Text { get; set; }

        public byte[] Image { get; set; }
    }
}
