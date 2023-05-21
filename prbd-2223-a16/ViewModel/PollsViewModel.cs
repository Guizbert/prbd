using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;
public class PollsViewModel : ViewModelCommon
{
    private ObservableCollection<PollsCardViewModel> _polls;

    public ObservableCollection<PollsCardViewModel> Polls {
        get => _polls;
        set=> SetProperty(ref _polls, value);
    }

    private string _filter;

    public string Filter {
        get => _filter;
        set=> SetProperty(ref _filter, value, ApplyFilterAction);
    }
    public ICommand ClearFilter { get; set; }
    public ICommand ApplyFilter { get; set; }
    public ICommand NewPoll{ get; set;}
    public ICommand DisplayPollsDetails { get; set;}



    public PollsViewModel() : base() {
        getPolls(); OnRefreshData();

        ApplyFilter = new RelayCommand(ApplyFilterAction);
        ClearFilter = new RelayCommand(() => Filter = "");
        NewPoll = new RelayCommand(() => {
            NotifyColleagues(App.Messages.MSG_CREATE_POLL, new Poll());
        });
        DisplayPollsDetails = new RelayCommand<PollsCardViewModel>(pc => {
            NotifyColleagues(App.Messages.MSG_CHOICE_POLL, pc.Poll);
        });
        //Register<Poll>(App.Messages.MSG_UPDATE_POLL, poll => ApplyFilterAction());
        Register<Poll>(App.Messages.MSG_UPDATE_POLL, poll => OnRefreshData());
    }

    private void getPolls() {

        IQueryable<Poll> polls = Poll.GetPolls(CurrentUser);

        Polls = new ObservableCollection<PollsCardViewModel>(
            polls.Select(p => new PollsCardViewModel(p)));
    }
    private void ApplyFilterAction() {

        if (!string.IsNullOrEmpty(Filter)) {
            IQueryable<Poll> query = Context.Polls;
            if (!CurrentUser.isAdmin()) // vérifiez si l'utilisateur est un administrateur
            {
                query = query.Where(p => p.Participants.Any(parti => parti.Id == CurrentUser.Id)); // filtrez les sondages en fonction de l'utilisateur connecté
            }

            query = query.Where(p => p.Name.Contains(Filter) || p.Participants.Any(parti => parti.FullName.Contains(Filter) || p.Creator.FullName.Contains(Filter))
            || p.Choices.Any(c => c.Label.Contains(Filter)));

            var filter = new ObservableCollection<PollsCardViewModel>(
                query.Select(p => new PollsCardViewModel(p)));

            Polls = filter;
        }
    }

    protected override void OnRefreshData() {
        getPolls();
    }
}

