using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

internal class PollDetailViewModel : ViewModelCommon {


    /* faire un notify colleague qui doit recevoir un choice quand on veut le delete 
     et dans le constructeur mettre un register choice qui dit
    'quand je recois le notify, je le remove de ma collection'
    */
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
        set => SetProperty(Poll.Name, value, Poll, (t, v) => {
            t.Name = v;
            //NotifyColleagues(App.Messages.MSG_UPDATE_POLL, Poll);
            NotifyColleagues(App.Messages.MSG_POLL_NAMECHANGED, Poll);
            //ajout validation
        });
    }
    private int _creatorId;

    public int CreatorId {
        get => CurrentUser.Id;
        set => SetProperty(ref _creatorId, value);
    }

    public string Creator {
        get => $"(Created by {CurrentUser.FullName})";
    }
    private bool _isClosed;
    public bool IsClosed {
        get => _isClosed;
        set => SetProperty(ref _isClosed, value);
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

    private User _selectedUserToDelete;

    public User SelectedUserToDelete {
        get => _selectedUserToDelete;
        set => SetProperty(ref _selectedUserToDelete, value);
    }

    private Choice _selectedChoice;

    public Choice SelectedChoice {
        get => _selectedChoice;
        set => SetProperty(ref _selectedChoice, value);
    }

    private ObservableCollection<User> _participants;

    public ObservableCollection<User> UserParticipants {
        get => _participants;
        set => SetProperty(ref _participants, value, () => AddParticipant());
    }

    private ObservableCollection<Choice> _choices;
    public ObservableCollection<Choice> Choices {
        get => _choices;
        set => SetProperty(ref _choices, value, () => AddChoice());
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
            Console.WriteLine("Ajout participant");
            UserParticipants.Add(SelectedUser); // Ajouter l'utilisateur sélectionné à la liste de participants
        } else {
            Console.WriteLine("peut pas l'ajouter");
        }
        Console.WriteLine("User Participants : ");
        foreach (User u in UserParticipants) {
            Console.WriteLine("   ----->  " + u.FullName);
        }
        refreshList();
    }

    private void AddChoice() {
        if (string.IsNullOrEmpty(ChoiceLabel)) {
            // Le libellé de choix est vide ou null, ne rien faire
            return;
        }
        if (Choices.Any(c => c.Label == ChoiceLabel)) {
            return;
        }else {
            var newChoice = new Choice { Label = ChoiceLabel };
         
            Choices.Add(newChoice);

            // Réinitialiser la valeur de la propriété ChoiceLabel
            ChoiceLabel = string.Empty;
            refreshList();
        }
    }

    private bool CanAddParticipant() {
        return SelectedUser != null && !UserParticipants.Any(p => p.Id == SelectedUser.Id); // Vérifier si l'utilisateur est sélectionné et n'est pas déjà dans la liste de participants
    }


    public PollDetailViewModel(Poll poll, bool isNew)
    {
        Poll = poll;
        IsNew = isNew;

        IsNotEditingChoice = true;
        IsEditingChoice = false;
        if(!IsNew) {
            UserParticipants = new ObservableCollection<User>(Poll.Participants.OrderBy(u => u.FullName));
            Choices = new ObservableCollection<Choice>(poll.Choices.OrderBy(c => c.Label));
            SetNbVoteUser();
        } else {
            UserParticipants = new ObservableCollection<User>();
           
            Choices = new ObservableCollection<Choice>();
        }
        IsClosed = Poll.Closed;
        SelectedPollType = Poll.Type;

        Save = new RelayCommand(SaveAction, CanSaveAction);
        Cancel = new RelayCommand(CancelAction, CanCancelAction);
        //Delete = new RelayCommand(DeleteAction, () => !IsNew);

        AddChoiceCommand = new RelayCommand(AddChoice);

        AddMyselfCommand = new RelayCommand(AddMyself);
        AddAllParticipantsCommand = new RelayCommand(AddAll);

        DeleteParticipantCommand = new RelayCommand<int>(DeleteParticipant);

        
        DeleteChoiceCommand = new RelayCommand<int>(DeleteChoice);
        UpdateChoiceCommand = new RelayCommand(UpdateChoice);

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
        }
        RaisePropertyChanged(nameof(UserParticipants));
    }
    public void AddAll() {
        foreach(var user in AllParticipants) {
            if (!UserParticipants.Contains(user)) {
                UserParticipants.Add(user);
            }
        }
        RaisePropertyChanged(nameof(UserParticipants));
    }
    private void DeleteParticipant(int userid) {
        var user = Poll.Participants.FirstOrDefault(u => u.Id == userid);
        if(user.NbVote > 0 ) {
            if(App.Confirm("you're about to delete the user " + user.FullName + " but he already voted in your poll. Are you sure?"))
                UserParticipants.Remove(user); // remove l'utilisateur sélectionné de la liste des participants
        } else {
            UserParticipants.Remove(user); 
        }
        refreshList();
    }

    private void UpdateChoice() {                               // <------ TODO  ------>
        IsNotEditingChoice = false;
        IsEditingChoice = true;
        Console.WriteLine(IsEditingChoice + " <- yeah");
    }
    private void DeleteChoice(int choiceId) {
        var choiceToDelete = Poll.Choices.FirstOrDefault(c => c.Id == choiceId);
        if(choiceToDelete.Votes.Count > 0) {
            if(App.Confirm("You're about to delete the choice " + choiceToDelete.Label + " But it already got some votes. Are you sure ?")) {
                Choices.Remove(choiceToDelete);
            }
        } else {
            Choices.Remove(choiceToDelete);
        }
        refreshList();

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
        if (IsNew) {
            var newPoll = new Poll(
                Title = Title,
                CreatorId = CreatorId,
                SelectedPollType = SelectedPollType,
                IsClosed = IsClosed);
            Context.Add(newPoll);
            Poll = newPoll; // pour récup l'id
            newPoll.Participants = UserParticipants;
            newPoll.Choices = Choices;
            IsNew = false;
        } else {
            //Poll.Participants = UserParticipants;
            //Poll.Choices = Choices;
            //Poll.Closed = IsClosed;
            Context.Update(Poll);
        }

        Context.SaveChanges();

        NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
        NotifyColleagues(App.Messages.MSG_UPDATE_POLL, Poll);
        NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
    }

    private bool CanSaveAction() {
        if (IsNew) {
            return
                !string.IsNullOrEmpty(Title) ;
        }
        return ( Poll.Name != null && 
            Poll.IsModified && (
            Poll.Participants != null &&
            Poll.Participants != UserParticipants) ||
            (Poll.Choices != null &&
            Poll.Choices != Choices));
    }

    public override void CancelAction() {
        if (IsNew) {
            IsNew = false;
        } else {
            Poll.Reload();
            RaisePropertyChanged();
        }
        NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
        NotifyColleagues(App.Messages.MSG_UPDATE_POLL, Poll);
        NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
    }

    private bool CanCancelAction() {
        return Poll != null && (IsNew || Poll.IsModified ||
            Poll.Participants != null ||
            Poll.Participants.Any() ||
            Poll.Choices != null ||
            Poll.Choices.Any());
    }
}

/*
 
 -> change vote fonctionne pas comprends pas pk


-> est ce que j'ai bien fait la récup des nbVote user / choice aussi par la même occasion

-> arrive pas a passer le textBox du comment en invisible



 */
