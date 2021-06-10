using System;
using System.Collections.Generic;
using System.Windows.Input;
using Factoriada.Models;
using Factoriada.Services.Interfaces;
using Factoriada.Utility;
using Xamarin.Forms;

namespace Factoriada.ViewModels
{
    public class RulesViewModel : ViewModelBase
    {
        #region Constructor
        public RulesViewModel(IDialogService dialogService, INavigationService navigationService, IApartmentService apartmentService) : base(dialogService, navigationService)
        {
            _apartmentService = apartmentService ?? throw new ArgumentNullException(nameof(apartmentService));

            InitializeCommands();
            Initialize();
        }
        #endregion

        #region Private Fields
        private readonly IApartmentService _apartmentService;
        private Guid _apartmentId;
        private List<Rule> _ruleList;
        private Rule _currentRule;
        private bool _userIsOwner;
        private bool _userIsOwnerAndSelected;
        #endregion

        #region Proprieties
        public bool UserIsOwnerAndSelected
        {
            get => _userIsOwnerAndSelected;
            set
            {
                _userIsOwnerAndSelected = value;
                OnPropertyChanged();
            }
        }
        public bool UserIsOwner
        {
            get => _userIsOwner;
            set
            {
                _userIsOwner = value;
                OnPropertyChanged();
            }
        }
        public Rule CurrentRule
        {
            get => _currentRule;
            set
            {
                _currentRule = value;
                UserIsOwnerAndSelected = value != null;
                OnPropertyChanged();
            }
        }
        public List<Rule> RuleList
        {
            get => _ruleList;
            set
            {
                _ruleList = value;
                OnPropertyChanged();
            }
        }
        public ICommand AddRuleCommand { get; set; }
        public ICommand EditRuleCommand { get; set; }
        public ICommand DeleteRuleCommand { get; set; }
        #endregion

        #region Private Methods
        private void InitializeCommands()
        {
            AddRuleCommand = new Command(AddRule);
            EditRuleCommand = new Command(EditRule);
            DeleteRuleCommand = new Command(DeleteRule);
        }
        private async void Initialize()
        {
            _dialogService.ShowLoading();
            UserIsOwner = ActiveUser.User.Role.RoleTypeName == "Owner";
            _apartmentId = await _apartmentService.GetApartmentIdByUser(ActiveUser.User.UserId);
            RuleList = await _apartmentService.GetRulesByApartmentId(_apartmentId);
            _dialogService.HideLoading();
        }
        private async void AddRule()
        {
            var result = await _dialogService.DisplayPromptAsync("Regula", "Introdu noua regula.");

            _dialogService.ShowLoading();

            var rule = new Rule { RuleId = Guid.NewGuid() };
            if (result == null)
                return;
            
            rule.RuleMessage = result;

            await _apartmentService.AddRuleToApartment(rule, _apartmentId);

            RuleList = await _apartmentService.GetRulesByApartmentId(_apartmentId);
            UserIsOwnerAndSelected = false;
            _dialogService.HideLoading();
        }
        private async void EditRule()
        {
            if (CurrentRule == null)
                return;

            var result = await _dialogService.DisplayPromptAsync("Regula", "Editeaza regula.", placeholder:CurrentRule.RuleMessage);

            _dialogService.ShowLoading();

            if (result == null)
                return;

            CurrentRule.RuleMessage = result;

            await _apartmentService.EditRuleFromApartment(CurrentRule);

            RuleList = await _apartmentService.GetRulesByApartmentId(_apartmentId);
            UserIsOwnerAndSelected = false;
            _dialogService.HideLoading();
        }
        private async void DeleteRule()
        {
            if (CurrentRule == null)
                return;

            _dialogService.ShowLoading();

            await _apartmentService.DeleteRule(CurrentRule);

            RuleList = await _apartmentService.GetRulesByApartmentId(_apartmentId);
            UserIsOwnerAndSelected = false;
            _dialogService.HideLoading();
        }
        #endregion
    }
}