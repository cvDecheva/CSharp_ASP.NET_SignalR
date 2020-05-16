using Chatapp.Client.Commands;
using Chatapp.Client.Models;
using Chatapp.Client.Services;
using Chatapp.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chatapp.Client.ViewModels
{
    public class SearchWindowViewModel : ViewModelBase
    {
        private IChatService chatService;

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
                List<User> users = await chatService.SearchAsync(MyProfile.Me.Username, _searchUsername);
                Participants = new ObservableCollection<Participant>();
                users.ForEach(u => Participants.Add(new Participant { Name = u.Username, Photo = u.Image }));
                return true;
            }
            catch (Exception) { return false; }
        }
        #endregion

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
              
                return true;
            }
            catch (Exception) { return false; }
        }
        #endregion

        #region Send Request Command
        private ICommand _sendRequestCommand;
        public ICommand SendRequestCommand
        {
            get
            {
                return _sendRequestCommand ?? (_sendRequestCommand = new RelayCommandAsync(() => SendRequest()));
            }
        }

        private async Task<bool> SendRequest()
        {
            try
            {
                await chatService.SendRequestAsync(MyProfile.Me.Username, SelectedParticipant.Name);

                return true;
            }
            catch (Exception) { return false; }
        }
        #endregion

        public SearchWindowViewModel(IChatService chatSvc)
        {
            chatService = chatSvc;

            Connect();               
        }
    }
}
