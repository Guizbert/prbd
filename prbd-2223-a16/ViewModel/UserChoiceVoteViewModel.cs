using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using FontAwesome6;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

                                                //====================== c'est la vue avec tous les choix possible ======================
public class UserChoiceVoteViewModel : ViewModelCommon
{

    public UserChoiceVoteViewModel(User user, Choice choice, Poll poll) {
        Poll = poll;
        UserVoted = user.Votes.Any(v => v.Choice.Id == choice.Id);
        UserVotedForCurrentPoll = user.Votes.Any(v => v.Choice.Poll == poll);
        _user = user;
        /* on doit récup a la creation de la vue les choix des users (si existant)*/
        Vote = user.Votes.FirstOrDefault(v =>v.Choice.Id == choice.Id)??new Vote() {Choice = choice, User = user, Type = VoteType.Empty };
        Console.WriteLine(Vote.Type + " LE VOTTEEEEEEEEEEEEEEEEEEEEEE");
        // faire un boolean pour savoir si le user a voté?
        ChangeVote = new RelayCommand<VoteType>(voteType => {
            var maxVote = Poll.GetMaxVotePossible();
            if (maxVote == 1) {
                if (UserVotedForCurrentPoll) {
                    return; // Si le nombre maximum de votes est 1 et le vote est déjà effectué par l'utilisateur, ne rien faire.
                }
            }
            if (CurrentUser.Id == user.Id || CurrentUser.Role == Role.Administrator && !UserVotedForCurrentPoll) {
                if (Vote.Type != VoteType.Empty && voteType != VoteType.Empty) {
                    Vote.Type = voteType;
                    Context.Update(Vote);
                } else if (Vote.Type != VoteType.Empty && voteType == VoteType.Empty) {
                    Context.Remove(Vote);
                } else if (Vote.Type == VoteType.Empty && voteType != VoteType.Empty) {
                    Vote.Type = voteType;
                    Context.Votes.Add(Vote);
                }
            }
            HasVotedNo = Vote.Type == VoteType.No;
            HasVotedYes = Vote.Type == VoteType.Yes;
            HasVotedMaybe = Vote.Type == VoteType.Maybe;
            NoVote = Vote.Type == VoteType.Empty; //fonctionne pas
            RaisePropertyChanged(nameof(GetCurrentIcon));
            RaisePropertyChanged(nameof(GetCurrentChoiceColor));
        });
        Console.WriteLine(ChangeVote);
        Console.WriteLine(GetCurrentIcon + "  <------------- CURRENTCOLOR ");

        HasVoted = user.Votes.Any(v => v.Choice.Id == choice.Id);
    }

    public Poll Poll { get; set; }
    public Vote Vote { get; private set; }
    public UserChoiceVoteViewModel() { }


    private User _user;
    public User User {
        get => _user;
    }
    private bool _editMode;
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value);
    }

    public ICommand ChangeVote { get; set; }

    public bool HasVoted {
        get;
        set;
    }

    private bool _hasVotedYes;
    public bool HasVotedYes {
        get => _hasVotedYes;
        set => SetProperty(ref _hasVotedYes, value);
    }

    public EFontAwesomeIcon CheckedIcon => HasVotedYes ? EFontAwesomeIcon.Solid_Check : EFontAwesomeIcon.None;
    public string YesToolTip => HasVotedYes ? "Yes" : "No";

    private bool _hasVotedNo;
    public bool HasVotedNo {
        get => _hasVotedNo;
        set => SetProperty(ref _hasVotedNo, value);
    }

    public EFontAwesomeIcon NoIcon => _hasVotedNo ? EFontAwesomeIcon.Solid_Xmark : EFontAwesomeIcon.None;
    public string NoToolTip => HasVotedNo ? "Yes" : "No";

    private bool _hasVotedMaybe;
    public bool HasVotedMaybe {
        get => _hasVotedMaybe;
        set => SetProperty(ref _hasVotedMaybe, value);
    }
    public EFontAwesomeIcon MaybeIcon => HasVotedMaybe ? EFontAwesomeIcon.Solid_CircleQuestion : EFontAwesomeIcon.None;

    public string MaybeToolTip => HasVotedMaybe ? "Yes" : "No";

    private bool _userVote;
    public bool UserVoted {
        get => _userVote;
        set => SetProperty(ref _userVote, value);
    }
    private bool _userVotedForCurrentPoll;
    public bool UserVotedForCurrentPoll {
        get => _userVotedForCurrentPoll;
        set => SetProperty(ref _userVotedForCurrentPoll, value);
    }

    private bool _noVote;
    public bool NoVote {
        get => _noVote;
        set => SetProperty(ref _noVote, value);
    }
    public Brush HasVotedYesColor => HasVotedYes ? Brushes.Green : Brushes.Gray;
    public Brush HasVotedNoColor => HasVotedYes ? Brushes.Red : Brushes.Gray;
    public Brush HasVotedMaybeColor => HasVotedYes ? Brushes.OrangeRed : Brushes.Gray;
    public Brush NoVoteColor => NoVote ? Brushes.White: Brushes.Gray;
    public EFontAwesomeIcon GetCurrentIcon => Vote.Type switch {
        VoteType.Yes => EFontAwesomeIcon.Solid_Check,
        VoteType.No => EFontAwesomeIcon.Solid_Xmark,
        VoteType.Maybe => EFontAwesomeIcon.Solid_CircleQuestion,
        _ => EFontAwesomeIcon.None,
    };

    public Brush GetCurrentChoiceColor => Vote.Type switch {
        VoteType.Yes => Brushes.Green,
        VoteType.No => Brushes.Red,
        VoteType.Maybe => Brushes.OrangeRed,
        _ => Brushes.White,
    };
   
  

    public void DeleteVotes() {

    }
    
}
