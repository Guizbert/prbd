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

    public ICommand Save { get; set; }
    public ICommand Cancel { get; set; }
    public ICommand AddMyselfCommand { get; set; }
    public ICommand AddAllParticipantsCommand { get; set; }
    public ICommand Delete { get; set; }
    public ICommand AddChoiceCommand {get;set;}
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
            NotifyColleagues(App.Messages.MSG_UPDATE_POLL, Poll);
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
        get => _isNew;
        set => SetProperty(ref _isClosed, value);
    }

    

    public static IEnumerable<User> AllParticipants {
        get {
            return Poll.AllUsers;
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

        var newChoice = new Choice { Label = ChoiceLabel };
        Choices.Add(newChoice);

        // Réinitialiser la valeur de la propriété ChoiceLabel
        ChoiceLabel = string.Empty;

        refreshList();

    }

    private bool CanAddParticipant() {
        return SelectedUser != null && !UserParticipants.Any(p => p.Id == SelectedUser.Id); // Vérifier si l'utilisateur est sélectionné et n'est pas déjà dans la liste de participants
    }


    public PollDetailViewModel(Poll poll, bool isNew)
    {
        Poll = poll;
        IsNew= isNew;
        if (UserParticipants == null) {
            UserParticipants = new ObservableCollection<User>();
        }
        if (Choices == null) {
            Choices = new ObservableCollection<Choice>();
        }
        Save = new RelayCommand(SaveAction, CanSaveAction);
        Cancel = new RelayCommand(CancelAction, CanCancelAction);
        Delete = new RelayCommand(DeleteAction, () => !IsNew);

        AddChoiceCommand = new RelayCommand(AddChoice);

        AddMyselfCommand = new RelayCommand(AddMyself);
        AddAllParticipantsCommand = new RelayCommand(AddAll);

        RaisePropertyChanged();
        refreshList();
    }
    public void refreshList() {
        //Poll.Participants = Participants;
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
 

    public override void SaveAction()
    {
        Console.WriteLine(Poll.Id + " 1 ");
        if (IsNew) {
            var newPoll = new Poll(
                Title = Title,
                CreatorId = CreatorId,
                SelectedPollType = SelectedPollType,
                IsClosed = IsClosed);
            Context.Add(newPoll);
            Context.SaveChanges();
            Console.WriteLine(newPoll.Id + "<---------------------------------------- 2 ");
            Poll = newPoll; // pour récup l'id

            IsNew = false;
        }

        Console.WriteLine(Poll.Id + "<------ 3 ");
        foreach (var user in UserParticipants) {
            var newParti = new Participation(Poll.Id, user.Id);
            Context.Participations.Add(newParti);
        }
        Context.SaveChanges();


        foreach (var choice in Choices) {
            var newChoice = new Choice(Poll.Id, choice.Label) ;
            Context.Choices.Add(newChoice);
        }
        Context.SaveChanges();
        

       

        NotifyColleagues(App.Messages.MSG_UPDATE_POLL, Poll);
    }

    private bool CanSaveAction() {
        if (IsNew) 
            return !string.IsNullOrEmpty(Title);
        return Poll != null && Poll.IsModified;

    }

    public override void CancelAction() {
        if (IsNew) {
            IsNew = false;
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
        } else {
            Poll.Reload();
            RaisePropertyChanged();
        }
    }

    private bool CanCancelAction() {
        return Poll != null && (IsNew || Poll.IsModified);
    }
    //private void DeleteAction() {
    //    //CancelAction();
    //    Poll.Delete();
    //    NotifyColleagues(App.Messages.MSG_UPDATE_POLL, Poll);
    //    NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
    //}

}

