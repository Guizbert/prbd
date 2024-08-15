using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;


namespace MyPoll.ViewModel;

class AdminViewModel : ViewModelCommon
{
    private User _user;
    public User User {
        get => _user;

        set => SetProperty(ref _user, value);
    }


    private User _selectedUser;

    public User SelectedUser {
        get => _selectedUser;
        set {
            _selectedUser = value;
            RaisePropertyChanged(nameof(SelectedUser));
        }
    }
    public ICommand e { get; set; }
    public ICommand SearchForPoll { get; set; }
    public ICommand DeleteUserFromPoll { get; set; }
    public ICommand DeleteUserFromPollParticipant { get; set; }

    
    public IEnumerable<User> Users { get; set; }
    public ObservableCollection<Poll> _polls;

    public ObservableCollection<Poll> Polls {
        get => _polls;
        set => SetProperty(ref _polls, value);
    }
    public ObservableCollection<Poll> _pollParticipant;

    public ObservableCollection<Poll> PollParticipant {
        get => _pollParticipant;
        set => SetProperty(ref _pollParticipant, value);
    }
    public AdminViewModel(User user) {
        User = user;
        Users = Context.Users.OrderBy(u => u.FullName).ToList();

        SearchForPoll = new RelayCommand(getPolls);
        DeleteUserFromPoll = new RelayCommand<int>(deleteUserFromPoll);
        DeleteUserFromPollParticipant = new RelayCommand<int>(deleteUserFromPollParticipant);
        /*
            list user + btn qui affiche tous les sondages où se trouve le user et on peut le delete
         */
    }

    private void getPolls() {
        if(SelectedUser != null) {
            IQueryable<Poll> polls = Poll.GetPollsWhereCreator(SelectedUser);
            Polls = new ObservableCollection<Poll>(polls.OrderBy(p => p.Name).ToList());
        }
        getPollsParticipants();
    }
    private void getPollsParticipants() {
        if(SelectedUser != null) {
            IQueryable<Poll> polls = Poll.GetPollsWhereParticipant(SelectedUser);
            PollParticipant = new ObservableCollection<Poll>(polls.OrderBy(p => p.Name).ToList());
        }
    }
    private void deleteUserFromPoll(int pollid)
    {
        var poll = Context.Polls.FirstOrDefault(p => p.Id == pollid);
        poll.Delete();
       

        NotifyColleagues(App.Messages.MSG_UPDATE_POLL, poll);
        getPolls();
    }
    private void deleteUserFromPollParticipant(int pollid)
    {
        var poll = Context.Polls.FirstOrDefault(p => p.Id == pollid);
        poll.Participants.Remove(SelectedUser);
        Context.SaveChanges();
        NotifyColleagues(App.Messages.MSG_UPDATE_POLL, poll);
        getPollsParticipants();
    }
}
