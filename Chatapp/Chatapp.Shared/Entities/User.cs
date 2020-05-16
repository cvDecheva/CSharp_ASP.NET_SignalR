using System;

namespace Chatapp.Shared.Entities
{
    public class User : Entity
    {
        public string ID { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime BirthDate { get; set; } = DateTime.Now;

        public byte[] Image { get; set; }

        public bool IsLoggedIn { get; set; }
    }
}