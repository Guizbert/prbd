using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using System.Windows.Media;
using MyPoll.Model;
using MyPoll.View;
using PRBD_Framework;
using Microsoft.IdentityModel.Tokens;

namespace MyPoll.ViewModel;

internal class PollDetailViewModel : ViewModelCommon {
    public ICommand Save { get; set; }
    public ICommand Cancel { get; set; }
    public ICommand AddMyselfCommand { get; set; }
    public ICommand AddAllParticipantsCommand { get; set; }
    public ICommand Delete { get; set; }
    public ICommand AddChoiceCommand {get;set;}


    public ICommand UpdateChoiceCommand { get; set; }
    public ICommand DeleteChoiceCommand { get; set;}
    public ICommand DeleteParticipantCommand { get; set;}

    private string _choiceLabel { get; set; }
    public string ChoiceLabel {
        get => _choiceLabel;
        set {
            if (!string.Equals(_choiceLabel, value)) {
                _choiceLabel = value;
                RaisePropertyChanged();
            }
        }
    }

    private ObservableCollectionFast<ChoiceViewModel> _choiceViewModel = new ObservableCollectionFast<ChoiceViewModel>();

    // Choice vm a la place de la view
    //la vue sera faite automatiquement
    //on aura directement accès au vm et on pourra appeler les choices

    public ObservableCollectionFast<ChoiceViewModel> ChoiceViewModel {
        get => _choiceViewModel;
        set => SetProperty(ref _choiceViewModel, value); 
    }


    private Poll _poll;
    public Poll Poll {
        get => _poll;
        set => SetProperty(ref _poll, value);
    }
    private bool _isNew;
    public bool IsNew {
        get => _isNew;
        set => SetProperty(ref _isNew, value);
    }
    public string Title {
        get => Poll.Name;
        set => SetProperty(Poll.Name, value, Poll, (p, v) => {
            p.Name = v;
            Validate();
            NotifyColleagues(App.Messages.MSG_POLL_NAMECHANGED, Poll);
            
        });
    }
    private int _creatorId;

    public int CreatorId {
        get => CurrentUser.Id;
        set => SetProperty(ref _creatorId, value);
    }
    public string Creator {
        get => $"Created by {(CurrentUser == Poll.Creator ? Poll.Creator.FullName : "Me (" +CurrentUser.FullName + ")")}";
    }
    private bool _isClosed;
    public bool IsClosed {
        get => _isClosed;
        set => SetProperty(ref _isClosed, value);
    }

    private bool _canChangeType;
    public bool CanChangeType {
        get => _canChangeType;
        set => SetProperty(ref _canChangeType, value);
    }

    public static IEnumerable<User> AllParticipants {
        get {
            return Poll.AllUsers;
        }
        set {
            AllParticipants= value;
        }
    }
    public static PollType[] getTypevalues => Poll.getTypes();

    private PollType _selectedPollType;

    public PollType SelectedPollType {
        get => _selectedPollType;
        set {
            _selectedPollType = value;
            ValidatePollType();
            RaisePropertyChanged(nameof(SelectedPollType));
        }
    }

    private User _selectedUser;

    public User SelectedUser {
        get => _selectedUser;
        set {
            _selectedUser = value;
            RaisePropertyChanged(nameof(SelectedUser));
        }
    }
    private ObservableCollection<User> _participants;

    public ObservableCollection<User> UserParticipants {
        get => _participants;
        set => SetProperty(ref _participants, value, () => AddParticipant());
    }

    private ObservableCollectionFast<Choice> _choices;
    public ObservableCollectionFast<Choice> Choices {
        get => _choices;
        set => SetProperty(ref _choices, value);
    }

    private bool _noParticipant;
    public bool NoParticipant {
        get => _noParticipant;
        set => SetProperty(ref _noParticipant, value, () => HasParticipant());
    }

    private bool _noChoices;
    public bool NoChoices {
        get => _noChoices;
        set => SetProperty(ref _noChoices, value, () => HasChoices());
    }

    private bool _isEditingChoice;
    public bool IsEditingChoice {
        get => _isEditingChoice;
        set => SetProperty(ref _isEditingChoice, value);
    }

    private bool _isNotEditingChoice;

    public bool IsNotEditingChoice {
        get => _isNotEditingChoice;
        set => SetProperty(ref _isNotEditingChoice, value);
    }


    private RelayCommand _addParticipantCommand;

    public ICommand AddParticipantCommand {
        get {
            if (_addParticipantCommand == null) {
                _addParticipantCommand = new RelayCommand(AddParticipant); 
            }
            return _addParticipantCommand;
        }
    }
    
    private void AddParticipant() {
        if (CanAddParticipant()) {
            Console.WriteLine("Ajout participant" + SelectedUser.FullName);
            UserParticipants.Add(SelectedUser); // Ajouter l'utilisateur sélectionné à la liste de participants
            SetNbVoteUser(SelectedUser);
            HasParticipant();
        } else {
            Console.WriteLine("peut pas l'ajouter");
            //ajout message erreur?
        }
        refreshList();
    }
    private bool CanAddParticipant() {
        return SelectedUser != null && !UserParticipants.Any(p => p.Id == SelectedUser.Id); // Vérifier si l'utilisateur est sélectionné et n'est pas déjà dans la liste de participants
    }

    /*  ======================================================== CONSTRUCTEUR ======================================================== */
    public PollDetailViewModel(Poll poll, bool isNew)
    {
        Poll = poll;
        IsNew = isNew;
        IsNotEditingChoice = true;
        IsEditingChoice = false;
        UserParticipants = new ObservableCollectionFast<User>(Poll.Participants.OrderBy(u => u.FullName));
        Choices = new ObservableCollectionFast<Choice>(poll.Choices.OrderBy(c => c.Label));
        HasParticipant();
        SetNbVoteUser();

        var listChoice = new ChoiceViewModel(Poll);
        ChoiceViewModel.Add(listChoice);


        //HasChoices();

        IsClosed = Poll.Closed;
        SelectedPollType = Poll.Type;

        Save = new RelayCommand(SaveAction, CanSaveAction);
        Cancel = new RelayCommand(CancelAction);
        //Delete = new RelayCommand(DeleteAction, () => !IsNew);


        AddMyselfCommand = new RelayCommand(AddMyself);
        AddAllParticipantsCommand = new RelayCommand(AddAll);
        DeleteParticipantCommand = new RelayCommand<int>(DeleteParticipant);
                
        //DeleteChoiceCommand = new RelayCommand<int>(DeleteChoice);

        RaisePropertyChanged();
        refreshList();
    }
    public PollDetailViewModel() {
    }

    public void refreshList() {
        //Poll.Participants = Participants;
        //UserParticipants.OrderBy(u => u.FullName);

        RaisePropertyChanged(nameof(UserParticipants));
        RaisePropertyChanged(nameof(Choices));
    }
    public void AddMyself() {
        if (!UserParticipants.Contains(CurrentUser)) {
            UserParticipants.Add(CurrentUser); // Ajouter l'utilisateur sélectionné à la liste de participants
            SetNbVoteUser(CurrentUser);
            if (NoParticipant) {
                NoParticipant = false;
                RaisePropertyChanged(nameof(NoParticipant));
            }
        }
        RaisePropertyChanged(nameof(UserParticipants));
    }
    public void AddAll() {
        foreach(var user in AllParticipants) {
            if (!UserParticipants.Contains(user)) {
                UserParticipants.Add(user);
                SetNbVoteUser(user);
            }
            HasParticipant();

        }
        RaisePropertyChanged(nameof(UserParticipants));
    }
    private void DeleteParticipant(int userid) {
        var user = UserParticipants.FirstOrDefault(u => u.Id == userid);
        var votesToDelete = Poll.Choices.SelectMany(c => c.Votes.Where(v => v.User == user)).ToList();
        Console.WriteLine(user.Id + " < - - - -");
        if(user.NbVote > 0 ) {
            if(App.Confirm("you're about to delete the user " + user.FullName + " but he already voted in your poll. Are you sure?")) {
                /* autre méthode que app.confirm
                 var result = MessageBox.Show("message ","Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                */
                UserParticipants.Remove(user); // remove l'utilisateur sélectionné de la liste des participants
                foreach (var vote in votesToDelete) {
                    Context.Votes.Remove(vote);
                }
            }
        } else {
            UserParticipants.Remove(user); 
        }
        HasParticipant();
        refreshList();
        RaisePropertyChanged();
    }

    public void SetNbVoteUser(User u) {
        u.NbVote = Choices.Sum(c => c.Votes.Count(v => v.UserId == u.Id));
    }

    public void SetNbVoteUser() {
       foreach(var u in UserParticipants) {
            SetNbVoteUser(u);
        }
    }

    public override void SaveAction()
    {
        if (Validate()) {
            if (IsNew) {

                var newPoll = new Poll(
                    Title = Title,
                    CreatorId = CreatorId,
                    SelectedPollType = SelectedPollType,
                    IsClosed = IsClosed);
                Context.Add(newPoll);
                Poll = newPoll; // pour récup l'id
                newPoll.Participants = UserParticipants;
                newPoll.Choices = ChoiceViewModel.SelectMany(c => c.Choices).ToList();
                IsNew = false;
            } else {
                Poll.Participants = UserParticipants;
                //Poll.Choices = Choices;
                Poll.Closed = IsClosed;
                Poll.Type = SelectedPollType;
                Context.Update(Poll);
            }

            Context.SaveChanges();

            NotifyColleagues(App.Messages.MSG_UPDATE_POLL, Poll);
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
        }

    }

    private bool CanSaveAction() {
        if (IsNew) {
            return !string.IsNullOrEmpty(Title) && !HasErrors && !NoParticipant;
        }
       
        return !HasErrors && !NoParticipant;
    }
   

    public override void CancelAction() {
        foreach (var entry in Context.ChangeTracker.Entries()) {
            switch (entry.State) {
                case EntityState.Modified:
                    entry.CurrentValues.SetValues(entry.OriginalValues);
                    entry.State = EntityState.Unchanged;
                    break;
                case EntityState.Added:
                    entry.State = EntityState.Detached;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Unchanged;
                    break;
            }
        }
        if (IsNew) {
            IsNew = false;
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, null);
        } else {
            Poll.Reload();
            Context.Entry(Poll).Reload();
            NotifyColleagues(App.Messages.MSG_POLL_NAMECHANGED, Poll);

        }
        NotifyColleagues(App.Messages.MSG_UPDATE_POLL, Poll);
        NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
    }



    // partie validation

    public override bool Validate() {
        ClearErrors();
        ValidateTitle();
        return !HasErrors;
    }

    public void HasChoices() {
        Console.WriteLine(ChoiceViewModel.All(cvm => cvm.Choices.Count == 0));
        NoChoices = ChoiceViewModel.All(cvm => cvm.Choices.Count == 0);
        RaisePropertyChanged(nameof(NoChoices));
    }

    public void HasParticipant() {
        NoParticipant = UserParticipants.IsNullOrEmpty();
        RaisePropertyChanged(nameof(NoParticipant));
    }

    public bool ValidateTitle() {
        if (string.IsNullOrEmpty(Title)) {
            AddError(nameof(Title), "Required");
        }
        if(Title.Length < 3) {
            AddError(nameof(Title), "Too short");
        }
        if (Title != Poll.Name) {
            if (CurrentUser.UserPolls.Any(p => p.Name == Title) ||
              CurrentUser.participatingIn.Any(p => p.Name == Title))
            {
                AddError(nameof(Title), "Already in a poll with the same name.");
            }
        }
        
        return !HasErrors;
    }
    public bool ValidatePollType() {

        bool hasMultipleVotes = UserParticipants.Any(user =>
        {
            var voteCount = Poll.Choices.Sum(c => c.Votes.Count(v => v.UserId == user.Id));
            return voteCount > 1;
        });
        CanChangeType = !hasMultipleVotes;

        return !HasErrors;
    }
   

}

