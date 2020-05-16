using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using System.Drawing;
using Chatapp.Client.Services;
using Chatapp.Client.Enums;
using Chatapp.Client.Models;
using Chatapp.Client.Commands;
using System.Windows.Input;
using System.Diagnostics;
using System.Reactive.Linq;
using Chatapp.Shared.Entities;
using System.Text.RegularExpressions;

namespace Chatapp.Client.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IChatService chatService;
        private IDialogService dialogService;
        private TaskFactory ctxTaskFactory;
        private const int MAX_IMAGE_WIDTH = 150;
        private const int MAX_IMAGE_HEIGHT = 150;

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        private User _user = new User();
        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<User> _friendshipRequests = new ObservableCollection<User>();
        public ObservableCollection<User> FriendshipRequests
        {
            get { return _friendshipRequests; }
            set
            {
                _friendshipRequests = value;
                OnPropertyChanged();
            }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        private string _realPassword = "";
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;

                if (_password.Length != 0)
                {
                    if (_password.Length == _realPassword.Length + 1)
                    {
                        _realPassword += _password[_password.Length - 1];
                    }
                    else
                    {
                        string temp = _realPassword;
                        _realPassword = "";
                        for (int i = 0; i < temp.Length - 1; i++)
                        {
                            _realPassword += temp[i];
                        }
                    }
                }
                else
                {
                    _realPassword = "";
                }

                _password = new String('*', _realPassword.Length);
                OnPropertyChanged();
            }
        }

        private string _profilePic;
        public string ProfilePic
        {
            get { return _profilePic; }
            set
            {
                _profilePic = value;
                OnPropertyChanged();
            }
        }

        private string _searchUsername;
        public string SearchUsername
        {
            get { return _searchUsername; }
            set
            {
                _searchUsername = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Participant> _participants = new ObservableCollection<Participant>();
        public ObservableCollection<Participant> Participants
        {
            get { return _participants; }
            set
            {
                _participants = value;
                OnPropertyChanged();
            }
        }

        private Participant _selectedParticipant;
        public Participant SelectedParticipant
        {
            get { return _selectedParticipant; }
            set
            {
                _selectedParticipant = value;
                if (SelectedParticipant.HasSentNewMessage) SelectedParticipant.HasSentNewMessage = false;
                OnPropertyChanged();
            }
        }

        private User _selectedUser;
        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
            }
        }

        private UserModes _userMode;
        public UserModes UserMode
        {
            get { return _userMode; }
            set
            {
                _userMode = value;
                OnPropertyChanged();
            }
        }

        private string _textMessage;
        public string TextMessage
        {
            get { return _textMessage; }
            set
            {
                _textMessage = value;
                OnPropertyChanged();
            }
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged();
            }
        }

        #region Connect Command
        private ICommand _connectCommand;
        public ICommand ConnectCommand
        {
            get
            {
                return _connectCommand ?? (_connectCommand = new RelayCommandAsync(() => Connect()));
            }
        }

        private async Task<bool> Connect()
        {
            try
            {
                await chatService.ConnectAsync();
                IsConnected = true;
                return true;
            }
            catch (Exception) { return false; }
        }
        #endregion

        #region Search Command
        private ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                return _searchCommand ?? (_searchCommand = new RelayCommandAsync(() => Search()));
            }
        }

        private async Task<bool> Search()
        {
            try
            {
                SearchWindow searchWindow = new SearchWindow();
                searchWindow.Show();

                return true;
            }
            catch (Exception) { return false; }
        }
        #endregion

        #region Login Command
        private ICommand _loginCommand;
        public ICommand LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand =
                    new RelayCommandAsync(() => Login(), (o) => CanLogin()));
            }
        }

        private async Task<bool> Login()
        {
            try
            {
                LoginData lg = await chatService.LoginAsync(_userName, _realPassword);

                if (lg.ErrorMessage != null)
                {
                    ErrorMessage = lg.ErrorMessage;
                    return false;
                }
                else if (lg.Friends != null)
                {
                    ErrorMessage = "";
                    lg.Friends.ForEach(u => Participants.Add(new Participant { Name = u.FriendInfo.Username, Photo = u.FriendInfo.Image, IsLoggedIn = u.FriendInfo.IsLoggedIn, Chatter = u.Messages }));

                    UserMode = UserModes.Chat;
                    IsLoggedIn = true;
                    User = lg.MyProfile;
                    MyProfile.Me = User;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private bool CanLogin()
        {
            return !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(UserName) && UserName.Length >= 3 && Password.Length >= 3 && IsConnected;
        }
        #endregion

        #region Register Command
        private ICommand _registerCommand;
        public ICommand RegisterCommand
        {
            get
            {
                return _registerCommand ?? (_registerCommand =
                    new RelayCommandAsync(() => Register(), (o) => CanRegister()));
            }
        }

        private async Task<bool> Register()
        {
            try
            {
                if (String.IsNullOrWhiteSpace(_user.LastName) || String.IsNullOrWhiteSpace(_user.Name) || String.IsNullOrWhiteSpace(_user.Password) || String.IsNullOrWhiteSpace(_user.Username) || String.IsNullOrWhiteSpace(_user.Email))
                {
                    ErrorMessage = "There is empty field!";
                    return false;
                }
                else if (!Regex.IsMatch(_user.LastName, @"^[а-яА-Яa-zA-Z]+$") || !Regex.IsMatch(_user.Name, @"^[а-яА-Яa-zA-Z]+$"))
                {
                    ErrorMessage = "The name and the last name\nmust contain only letters!";
                    return false;
                }
                else if (_user.Username.Length < 3 || _user.Password.Length < 3)
                {
                    ErrorMessage = "The username and password\ncan't be less than 3 letters!";
                    return false;
                }
                else if (!Regex.IsMatch(_user.Username, @"^[a-zA-Z0-9_]+$") || !Regex.IsMatch(_user.Password, @"^[a-zA-Z0-9_]+$"))
                {
                    ErrorMessage = "The username and password must\nbe only latin letters, _ and numbers!";
                    return false;
                }
                else if (!Regex.IsMatch(_user.Email, @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$"))
                {
                    ErrorMessage = "The email isn't correct!";
                    return false;
                }
                else if (_user.BirthDate.Year < 1900)
                {
                    ErrorMessage = "The birth year must be after 1900!";
                    return false;
                }

                _user.Image = Avatar();

                if (_user.Image == null)
                {
                    _user.Image = new byte[0];
                }

                LoginData lg = await chatService.RegisterAsync(_user);
                if (lg.ErrorMessage != null)
                {
                    ErrorMessage = lg.ErrorMessage;
                    return false;
                }
                else if (lg.Friends != null)
                {
                    lg.Friends.ForEach(u => Participants.Add(new Participant { Name = u.FriendInfo.Username, Photo = u.FriendInfo.Image }));
                    UserMode = UserModes.Chat;
                    IsLoggedIn = true;
                    return true;
                }
                else
                {
                    dialogService.ShowNotification("Username is already in use");
                    return false;
                }

            }
            catch (Exception) { return false; }
        }

        private bool CanRegister()
        {
            return true;
        }
        #endregion

        #region Logout Command
        private ICommand _logoutCommand;
        public ICommand LogoutCommand
        {
            get
            {
                return _logoutCommand ?? (_logoutCommand =
                    new RelayCommandAsync(() => Logout(), (o) => CanLogout()));
            }
        }

        private async Task<bool> Logout()
        {
            try
            {
                await chatService.LogoutAsync();
                await chatService.LogoutAsync();
                UserMode = UserModes.Login;
                UserName = "";
                Password = "";
                User = null;
                Participants = new ObservableCollection<Participant>();
                return true;
            }
            catch (Exception) { return false; }
        }

        private bool CanLogout()
        {
            return IsConnected && IsLoggedIn;
        }
        #endregion

        #region Typing Command
        private ICommand _typingCommand;
        public ICommand TypingCommand
        {
            get
            {
                return _typingCommand ?? (_typingCommand =
                    new RelayCommandAsync(() => Typing(), (o) => CanUseTypingCommand()));
            }
        }

        private async Task<bool> Typing()
        {
            try
            {
                await chatService.TypingAsync(SelectedParticipant.Name);
                return true;
            }
            catch (Exception) { return false; }
        }

        private bool CanUseTypingCommand()
        {
            return (SelectedParticipant != null && SelectedParticipant.IsLoggedIn);
        }
        #endregion

        #region Accept Friend Command
        private ICommand _acceptCommand;
        public ICommand AcceptCommand
        {
            get
            {
                return _acceptCommand ?? (_acceptCommand = new RelayCommandAsync(() => Accept()));
            }
        }

        private async Task<bool> Accept()
        {
            try
            {
                if (SelectedUser == null) return false;

                User u = FriendshipRequests.FirstOrDefault((fr) => fr.Username == SelectedUser.Username);

                if (u == null) return false;

                FriendshipRequests.Remove(u);

                await chatService.FriendshipRequestAnswerAsync(u, true);
                return true;
            }
            catch (Exception) { return false; }
        }
        #endregion

        #region Accept Friend Command
        private ICommand _declineCommand;
        public ICommand DeclineCommand
        {
            get
            {
                return _declineCommand ?? (_declineCommand = new RelayCommandAsync(() => Decline()));
            }
        }

        private async Task<bool> Decline()
        {
            try
            {
                if (SelectedUser == null) return false;

                User u = FriendshipRequests.FirstOrDefault((fr) => fr.Username == SelectedUser.Username);

                if (u == null) return false;

                FriendshipRequests.Remove(u);

                await chatService.FriendshipRequestAnswerAsync(u, false);
                return true;
            }
            catch (Exception) { return false; }
        }
        #endregion

        #region Send Text Message Command
        private ICommand _sendTextMessageCommand;
        public ICommand SendTextMessageCommand
        {
            get
            {
                return _sendTextMessageCommand ?? (_sendTextMessageCommand =
                    new RelayCommandAsync(() => SendTextMessage(), (o) => CanSendTextMessage()));
            }
        }

        private async Task<bool> SendTextMessage()
        {
            try
            {
                var recepient = _selectedParticipant.Name;
                await chatService.SendUnicastMessageAsync(recepient, _textMessage);
                return true;
            }
            catch (Exception) { return false; }
            finally
            {
                ChatMessage msg = new ChatMessage
                {
                    Author = UserName,
                    Message = _textMessage,
                    Time = DateTime.Now,
                    IsOriginNative = true
                };
                SelectedParticipant.Chatter.Add(msg);
                TextMessage = string.Empty;
            }
        }

        private bool CanSendTextMessage()
        {
            return (!string.IsNullOrEmpty(TextMessage) && IsConnected && _selectedParticipant != null);
        }
        #endregion

        #region Send Picture Message Command
        private ICommand _sendImageMessageCommand;
        public ICommand SendImageMessageCommand
        {
            get
            {
                return _sendImageMessageCommand ?? (_sendImageMessageCommand =
                    new RelayCommandAsync(() => SendImageMessage(), (o) => CanSendImageMessage()));
            }
        }

        private async Task<bool> SendImageMessage()
        {
            string pic = dialogService.OpenFile("Select image file", "Images (*.jpg;*.png)|*.jpg;*.png");
            if (string.IsNullOrEmpty(pic)) return false;

            var image = Image.FromFile(pic);
            if (image.Width > MAX_IMAGE_WIDTH || image.Height > MAX_IMAGE_HEIGHT)
            {
                dialogService.ShowNotification($"Image size should be {MAX_IMAGE_WIDTH} x {MAX_IMAGE_HEIGHT} or less.");
                return false;
            }

            var img = await Task.Run(() => File.ReadAllBytes(pic));

            try
            {
                var recepient = _selectedParticipant.Name;
                await chatService.SendUnicastMessageAsync(recepient, img);
                return true;
            }
            catch (Exception) { return false; }
            finally
            {
                ChatMessage msg = new ChatMessage { Author = UserName, Picture = pic, Time = DateTime.Now, IsOriginNative = true };
                SelectedParticipant.Chatter.Add(msg);
            }
        }

        private bool CanSendImageMessage()
        {
            return (IsConnected && _selectedParticipant != null);
        }
        #endregion

        #region Select Profile Picture Command
        private ICommand _selectProfilePicCommand;
        public ICommand SelectProfilePicCommand
        {
            get
            {
                return _selectProfilePicCommand ?? (_selectProfilePicCommand =
                    new RelayCommand((o) => SelectProfilePic()));
            }
        }

        private void SelectProfilePic()
        {
            var pic = dialogService.OpenFile("Select image file", "Images (*.jpg;*.png)|*.jpg;*.png");
            if (!string.IsNullOrEmpty(pic))
            {
                var img = Image.FromFile(pic);
                if (img.Width > MAX_IMAGE_WIDTH || img.Height > MAX_IMAGE_HEIGHT)
                {
                    dialogService.ShowNotification($"Image size should be {MAX_IMAGE_WIDTH} x {MAX_IMAGE_HEIGHT} or less.");
                    return;
                }
                ProfilePic = pic;
            }
        }
        #endregion

        #region Open Image Command
        private ICommand _openImageCommand;
        public ICommand OpenImageCommand
        {
            get
            {
                return _openImageCommand ?? (_openImageCommand =
                    new RelayCommand<ChatMessage>((m) => OpenImage(m)));
            }
        }

        private void OpenImage(ChatMessage msg)
        {
            var img = msg.Picture;
            if (string.IsNullOrEmpty(img) || !File.Exists(img)) return;
            Process.Start(img);
        }
        #endregion

        #region Change View Command
        private ICommand _changeViewCommand;
        public ICommand ChangeViewCommand
        {
            get
            {
                return _changeViewCommand ?? (_changeViewCommand =
                    new RelayCommand((o) => ChangeView()));
            }
        }

        private void ChangeView()
        {
            ErrorMessage = "";

            switch (UserMode)
            {
                case UserModes.Login:
                    UserMode = UserModes.Registration;
                    break;
                case UserModes.Registration:
                    UserMode = UserModes.Login;
                    break;
                case UserModes.Chat:
                    UserMode = UserModes.Login;
                    break;
            }
        }
        #endregion

        #region Event Handlers
        private void NewTextMessage(string name, string msg, MessageType mt)
        {
            if (mt == MessageType.Unicast)
            {
                ChatMessage cm = new ChatMessage { Author = name, Message = msg, Time = DateTime.Now };
                var sender = _participants.Where((u) => string.Equals(u.Name, name)).FirstOrDefault();
                ctxTaskFactory.StartNew(() => sender.Chatter.Add(cm)).Wait();

                if (!(SelectedParticipant != null && sender.Name.Equals(SelectedParticipant.Name)))
                {
                    ctxTaskFactory.StartNew(() => sender.HasSentNewMessage = true).Wait();
                }
            }
        }

        private void NewImageMessage(string name, byte[] pic, MessageType mt)
        {
            if (mt == MessageType.Unicast)
            {
                var imgsDirectory = Path.Combine(Environment.CurrentDirectory, "Image Messages");
                if (!Directory.Exists(imgsDirectory)) Directory.CreateDirectory(imgsDirectory);

                var imgsCount = Directory.EnumerateFiles(imgsDirectory).Count() + 1;
                var imgPath = Path.Combine(imgsDirectory, $"IMG_{imgsCount}.jpg");

                ImageConverter converter = new ImageConverter();
                using (Image img = (Image)converter.ConvertFrom(pic))
                {
                    img.Save(imgPath);
                }

                ChatMessage cm = new ChatMessage { Author = name, Picture = imgPath, Time = DateTime.Now };
                var sender = _participants.Where(u => string.Equals(u.Name, name)).FirstOrDefault();
                ctxTaskFactory.StartNew(() => sender.Chatter.Add(cm)).Wait();

                if (!(SelectedParticipant != null && sender.Name.Equals(SelectedParticipant.Name)))
                {
                    ctxTaskFactory.StartNew(() => sender.HasSentNewMessage = true).Wait();
                }
            }
        }

        private void NewFriendshipRequest(User user, MessageType mt)
        {
            if (mt == MessageType.Unicast)
            {
                ctxTaskFactory.StartNew(() => FriendshipRequests.Add(user)).Wait();
            }
        }

        private void NewFriendshipAdd(User user, MessageType mt)
        {
            if (mt == MessageType.Unicast)
            {
                ctxTaskFactory.StartNew(() => Participants.Add(new Participant { Name = user.Username, Photo = user.Image, IsLoggedIn = user.IsLoggedIn })).Wait();
            }
        }

        private void ParticipantLogin(User u)
        {
            Participant ptp = Participants.FirstOrDefault(p => string.Equals(p.Name, u.Username));
            if (_isLoggedIn && ptp != null)
            {
                ptp.IsLoggedIn = true;
            }
        }

        private void ParticipantDisconnection(string name)
        {
            var person = Participants.Where((p) => string.Equals(p.Name, name)).FirstOrDefault();
            if (person != null) person.IsLoggedIn = false;
        }

        private void ParticipantReconnection(string name)
        {
            var person = Participants.Where((p) => string.Equals(p.Name, name)).FirstOrDefault();
            if (person != null) person.IsLoggedIn = true;
        }

        private void Reconnecting()
        {
            IsConnected = false;
            IsLoggedIn = false;
        }

        private async void Reconnected()
        {
            var pic = Avatar();
            if (!string.IsNullOrEmpty(_userName)) await chatService.LoginAsync(_userName, _password);
            IsConnected = true;
            IsLoggedIn = true;
        }

        private async void Disconnected()
        {
            var connectionTask = chatService.ConnectAsync();
            await connectionTask.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    IsConnected = true;
                    chatService.LoginAsync(_userName, _password).Wait();
                    IsLoggedIn = true;
                }
            });
        }

        private void ParticipantTyping(string name)
        {
            var person = Participants.Where((p) => string.Equals(p.Name, name)).FirstOrDefault();
            if (person != null && !person.IsTyping)
            {
                person.IsTyping = true;
                Observable.Timer(TimeSpan.FromMilliseconds(1500)).Subscribe(t => person.IsTyping = false);
            }
        }
        #endregion

        private byte[] Avatar()
        {
            byte[] pic = null;
            if (!string.IsNullOrEmpty(_profilePic)) pic = File.ReadAllBytes(_profilePic);
            return pic;
        }

        public MainWindowViewModel(IChatService chatSvc, IDialogService diagSvc)
        {
            dialogService = diagSvc;
            chatService = chatSvc;

            chatSvc.NewTextMessage += NewTextMessage;
            chatSvc.NewImageMessage += NewImageMessage;
            chatSvc.NewFriendshipRequest += NewFriendshipRequest;
            chatSvc.NewFriendshipAdd += NewFriendshipAdd;
            chatSvc.ParticipantLoggedIn += ParticipantLogin;
            chatSvc.ParticipantLoggedOut += ParticipantDisconnection;
            chatSvc.ParticipantDisconnected += ParticipantDisconnection;
            chatSvc.ParticipantReconnected += ParticipantReconnection;
            chatSvc.ParticipantTyping += ParticipantTyping;
            chatSvc.ConnectionReconnecting += Reconnecting;
            chatSvc.ConnectionReconnected += Reconnected;
            chatSvc.ConnectionClosed += Disconnected;

            ctxTaskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
        }

    }
}
