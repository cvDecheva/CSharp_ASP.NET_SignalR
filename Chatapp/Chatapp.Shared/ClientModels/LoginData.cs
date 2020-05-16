using Chatapp.Client.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Chatapp.Shared.Entities
{
    public class LoginData
    {
        public List<Friend> Friends { get; set; }

        public User MyProfile { get; set; }

        public string ErrorMessage { get; set; }

        public LoginData(List<Friend> friends, User myProfile, string errorMessage = null)
        {
            Friends = friends;
            MyProfile = myProfile;
            ErrorMessage = errorMessage;
        }
    }

    public class Friend
    { 
        public User FriendInfo { get; set; }

        public ObservableCollection<ChatMessage> Messages { get; set; }
    }
}
