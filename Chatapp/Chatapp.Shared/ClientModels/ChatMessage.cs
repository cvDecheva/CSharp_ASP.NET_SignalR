using System;

namespace Chatapp.Client.Models
{
    public class ChatMessage
    {
        public string Message { get; set; }
        public string Author { get; set; }
        public DateTime Time { get; set; }
        public string Picture { get; set; }
        public byte[] PictureData { get; set; }
        public bool IsOriginNative { get; set; }
    }
}
