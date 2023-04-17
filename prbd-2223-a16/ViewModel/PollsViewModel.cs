using System.Collections.ObjectModel;
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
        set=> SetProperty(ref _filter, value);
    }
    public ICommand ClearFilter { get; set; }
    public ICommand NewPoll{ get; set;}



    public PollsViewModel() : base() {
        ApplyFilterAction();
        ClearFilter = new RelayCommand(() => Filter = "");

        NewPoll = new RelayCommand(() => {
            NotifyColleagues(App.Messages.MSG_CREATE_POLL, new Poll());
        });
        Register<Poll>(App.Messages.MSG_UPDATE_POLL, poll => ApplyFilterAction());

    }

    private void ApplyFilterAction() {
        IEnumerable<Poll> query = Context.Polls.OrderBy(p => p.Id);
        if(!string.IsNullOrEmpty(Filter) ) {
            query = from p in Context.Polls
                    where p.Name.Contains(Filter)
                    orderby p.Name
                    select p;
        }
        Polls = new ObservableCollection<PollsCardViewModel>(query.Select(p => new PollsCardViewModel(p)));

    }
    protected override void OnRefreshData() {
        // pour plus tard
    }
}

