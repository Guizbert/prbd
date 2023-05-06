using System.Windows.Input;
using System.Windows.Media;
using FontAwesome6;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

                                                //====================== c'est la vue avec tous les choix possible ======================
public class UserChoiceVoteViewModel : ViewModelCommon
{
    public UserChoiceVoteViewModel(User user, Choice choice) {
        UserVote = user.Votes.Any(v => v.Choice.Id == choice.Id);

        /* on doit récup a la creation de la vue les choix des users (si existant)*/
        Vote = user.Votes.FirstOrDefault(v =>v.Choice.Id == choice.Id, new Vote() {Choice = choice, User = user });

        // faire un boolean pour savoir si le user a voté? 
        ChangeVote = new RelayCommand(() => HasVotedYes = !HasVotedYes);
    }

    public Vote Vote { get; private set; }
    public UserChoiceVoteViewModel() { }

    private bool _editMode;
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value);
    }

    public ICommand ChangeVote { get; set; }

    private bool _hasVotedYes;
    public bool HasVotedYes {
        get => _hasVotedYes;
        set => SetProperty(ref _hasVotedYes, value);
    }

    public EFontAwesomeIcon CheckedIcon => HasVotedYes ? EFontAwesomeIcon.Solid_Check : EFontAwesomeIcon.None;
    public Brush CheckedIconColor => HasVotedYes ? Brushes.Green : Brushes.White;

    private bool _hasVotedNo;
    public bool HasVotedNo {
        get => _hasVotedNo;
        set => SetProperty(ref _hasVotedNo, value);
    }

    public EFontAwesomeIcon NoIcon => _hasVotedNo ? EFontAwesomeIcon.Solid_Xmark : EFontAwesomeIcon.None;
    public Brush NoIconColor => HasVotedNo ? Brushes.Red : Brushes.White;


    private bool _hasVotedMaybe;
    public bool HasVotedMaybe {
        get => _hasVotedMaybe;
        set => SetProperty(ref _hasVotedMaybe, value);
    }

    public EFontAwesomeIcon MaybedIcon => HasVotedMaybe ? EFontAwesomeIcon.Solid_CircleQuestion : EFontAwesomeIcon.None;
    public Brush MaybedIconColor => HasVotedNo ? Brushes.Yellow : Brushes.White;

    private bool _userVote;
    public bool UserVote {
        get => _userVote;
        set => SetProperty(ref _userVote, value);
    }
}
