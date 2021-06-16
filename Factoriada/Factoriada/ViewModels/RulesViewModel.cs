using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
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
                if (UserIsOwner && value != null)
                    UserIsOwnerAndSelected = true;
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
            _apartmentId = ActiveUser.ApartmentGuid.ApartmentDetailId;
            RuleList = await _apartmentService.GetRulesByApartmentId(_apartmentId);
            _dialogService.HideLoading();
        }
        private async void AddRule()
        {
            var result = await _dialogService.DisplayPromptAsync("Regula", "Introdu noua regula.");

            if (result == null)
                return;

            _dialogService.ShowLoading();

            var rule = new Rule {RuleId = Guid.NewGuid(), RuleMessage = result, InsertedDateTime = DateTime.Now};

            await _apartmentService.AddRuleToApartment(rule, _apartmentId);
            RuleList.Add(rule);
            RuleList = new List<Rule>(RuleList);

            UserIsOwnerAndSelected = false;

            _dialogService.HideLoading();
        }
        private async void EditRule()
        {
            if (CurrentRule == null)
                return;

            var result = await _dialogService.DisplayPromptAsync("Regula", "Editeaza regula.", placeholder:CurrentRule.RuleMessage);

            if (result == null)
                return;

            _dialogService.ShowLoading();

            CurrentRule.RuleMessage = result;

            await _apartmentService.EditRuleFromApartment(CurrentRule);
            RuleList
                .Select(x =>
                {
                    if (x.ApartmentDetail == CurrentRule.ApartmentDetail)
                        x.RuleMessage = CurrentRule.RuleMessage;

                    return x;
                });
            RuleList = new List<Rule>(RuleList);

            CurrentRule = null;
            UserIsOwnerAndSelected = false;
            _dialogService.HideLoading();
        }
        private async void DeleteRule()
        {
            if (CurrentRule == null)
                return;

            _dialogService.ShowLoading();

            await _apartmentService.DeleteRule(CurrentRule);

            RuleList.Remove(CurrentRule);
            RuleList = new List<Rule>(RuleList);
            UserIsOwnerAndSelected = false;
            CurrentRule = null;
            _dialogService.HideLoading();
        }
        #endregion
    }
}