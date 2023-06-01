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
        Choice = choice;
        /* on doit récup a la creation de la vue les choix des users (si existant)*/
        Vote = user.Votes.FirstOrDefault(v =>v.Choice.Id == choice.Id)??new Vote() {Choice = choice, User = user, Type = VoteType.Empty };
        // pour pouvoir change les vote :
        ChangeVote = new RelayCommand<VoteType>(voteType => {
            var max = Poll.Type == PollType.Single ? 1 : 0;
            if (CurrentUser.Id == user.Id || CurrentUser.Role == Role.Administrator ) {
                if (max == 1) {
                     ChangeVotePollSingle(voteType);
                        //NotifyColleagues(App.Messages.MSG_NEWCHOICE_POLLSINGLE, voteType);
                        // Si le nombre maximum de votes est 1 et le vote est déjà effectué par l'utilisateur, ne rien faire.
                        //p'tetre faire un notify colleague pour delete les votes
                }
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
            Console.WriteLine(VoteColor + "   color a afficher quand on edit ");
            //NoVote = Vote.Type == VoteType.Empty; 
            RaisePropertyChanged(nameof(GetCurrentIcon));
            RaisePropertyChanged(nameof(GetCurrentChoiceColor));
                
        });
        _voteColor = GetVoteColor();
        RaisePropertyChanged(nameof(VoteColor));

        Console.WriteLine(ChangeVote);
        Console.WriteLine(GetCurrentIcon + "  <------------- CURRENTCOLOR ");

        NotifyColleagues(App.Messages.MSG_UPDATE_POLL, Poll);
        HasVoted = user.Votes.Any(v => v.Choice.Id == choice.Id);
    }
    public ICommand ChangeVote { get; set; }

    public Poll Poll { get; set; }
    public Vote Vote { get;  set; }

    Brush _voteColor;

    public Brush VoteColor => GetVoteColor();

    public UserChoiceVoteViewModel() { }


    private User _user;
    public User User {
        get => _user;
    }

    private Choice _choice;
    public Choice Choice {
        get => _choice;
        set => SetProperty(ref _choice, value);
    }
    private bool _editMode;
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value);
    }
    private void ChangeVotePollSingle(VoteType type) {
        if (VoteGridVm == null) VoteGridVm = new VoteGridViewModel(Poll);
        if (type == VoteType.Empty)
            return;
        else
            Vote.Type = type;
        
                
        NotifyColleagues(App.Messages.MSG_UPDATE_VOTE, VoteGridVm);


        if (VoteGridVm == null || VoteGridVm.Poll.Type != PollType.Single || Vote.Type == VoteType.Empty) return;

        var vpm = VoteGridVm.Participants.Find(u => u.Id == User.Id);

        foreach (var c in vpm.Votes.Where(v => v.Choice.Id != Choice.Id && v.Choice.Poll == Poll)) {
            //c.Type = VoteType.Empty;
            Context.Remove(c);
        }
        
    }


    private VoteGridViewModel _voteVm;
    public VoteGridViewModel VoteGridVm {
        get => _voteVm;
        set => SetProperty(ref _voteVm, value);
    }

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

    Brush GetVoteColor() {
        //Console.WriteLine("yes  : " + HasVotedYes + "\n" + "Maybe : " + HasVotedMaybe + "\n No : " + HasVotedNo);
        if (HasVotedYes) return Brushes.Green;
        if (HasVotedMaybe) return Brushes.OrangeRed;
        if (HasVotedNo) return Brushes.Red;
        else
            return Brushes.Gray;
    }

    public Brush GetCurrentChoiceColor => Vote.Type switch {
        VoteType.Yes => Brushes.Green,
        VoteType.No => Brushes.Red,
        VoteType.Maybe => Brushes.OrangeRed,
        _ => Brushes.White,
    };
    
}
