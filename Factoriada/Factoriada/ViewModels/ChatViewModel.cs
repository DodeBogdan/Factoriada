using System;
using System.Collections.Generic;
using System.Windows.Input;
using Factoriada.Models;
using Factoriada.Services.Interfaces;
using Factoriada.Utility;
using Xamarin.Forms;

namespace Factoriada.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        public ChatViewModel(IDialogService dialogService, INavigationService navigationService, IApartmentService apartmentService) : base(dialogService, navigationService)
        {
            _apartmentService = apartmentService;
            Initialize();
            InitializeCommands();
        }

        #region Private Fields
        private readonly IApartmentService _apartmentService;
        private Guid _currentApartment;
        private List<Chat> _chatList;
        private string _chatEntry;

        #endregion

        #region Proprieties

        public string ChatEntry
        {
            get => _chatEntry;
            set
            {
                _chatEntry = value;
                OnPropertyChanged();
            }
        }

        public List<Chat> ChatList
        {
            get => _chatList;
            set
            {
                _chatList = value;
                OnPropertyChanged();
            }
        }

        public ICommand SendMessageCommand { get; set; }
        #endregion

        #region Private Methods
        private void InitializeCommands()
        {
            SendMessageCommand = new Command(SendMessage);

        }

        private async void Initialize()
        {
            _currentApartment = await _apartmentService.GetApartmentIdByUser(ActiveUser.User.UserId);
            ChatList = await _apartmentService.GetChatByApartmentId(_currentApartment);
        }

        private async void SendMessage()
        {
            if (string.IsNullOrEmpty(ChatEntry))
                return;

            var chat = new Chat()
            {
                ChatId = Guid.NewGuid(),
                DateTime = DateTime.Now,
                User = ActiveUser.User,
                ApartmentId = _currentApartment,
                Message = ChatEntry
            };

            await _apartmentService.SendMessage(chat);

            ChatEntry = "";
            ChatList = await _apartmentService.GetChatByApartmentId(_currentApartment);
        }

        #endregion
    }
}