using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class UserChoiceViewModel : ViewModelCommon
{

    public UserChoiceViewModel(VoteGridViewModel voteGridViewModel, User user, List<Choice> choices) {
        _voteGridViewModel = voteGridViewModel;

        _choices = choices;
        _choicesSave = _choices;

        Poll = voteGridViewModel.Poll;
        User = user;
        RefreshVote();

        EditCommand = new RelayCommand(() => EditMode = true);
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
        DeleteCommand = new RelayCommand(Delete);
        /*
                verif ici le mode single :
                        -> rechercher dans le gridvm dans userVm rechercher le participant courant parcourir ses choix et changer ses votes et mettre a empty
         */

    }
    
    public Poll Poll { get; set ; }

    private VoteGridViewModel _voteGridViewModel;
    public ICommand EditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand DeleteCommand { get; }

    private List<Choice> _choices;
    private List<Choice> _choicesSave;

    public User User { get; }

    public VoteType Vote { get; set; }

    private bool _editMode;

    public bool IsActionVisible => !_voteGridViewModel.Poll.Closed &&( CurrentUser == User || CurrentUser.Role == Role.Administrator);

    // La visbilité des boutons de sauvegarde et d'annulation sont bindés sur cette propriété
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value, EditModeChanged);
    }

    private void EditModeChanged() {
        // Lorsqu'on change le mode d'édition de la ligne, on le signale à chaque cellule
        foreach (UserChoiceVoteViewModel voteVM in _voteVM) {
            voteVM.EditMode = EditMode;
        }

        // On informe le parent qu'on change le mode d'édition de la ligne
        _voteGridViewModel.AskEditMode(EditMode);
    }

    // Méthode appelée par le VM parent pour que la ligne mettre à jour la visibilité des boutons
    public void Changes() {
        RaisePropertyChanged(nameof(Editable));
    }


    // La ligne est éditable si elle n'est pas déjà en mode édition et si aucune autre ligne ne l'est
    // On récupére cette info via ParentEditMode
    // La visbilité des boutons d'édition et de suppression sont bindés sur cette propriété
    public bool Editable => !EditMode && !ParentEditMode;

    public bool ParentEditMode => _voteGridViewModel.EditMode;


    private List<UserChoiceVoteViewModel> _voteVM = new();
    public List<UserChoiceVoteViewModel> VoteVM {
        get => _voteVM;
        private set => SetProperty(ref _voteVM, value);
    }

    private void RefreshVote() {
        VoteVM = _choices
            .Select(c => new UserChoiceVoteViewModel(User, c, Poll))        // mettre la vue avec les choix
            .ToList();
        // faire une fonction en db en utilisant la db
    }

    private void RefreshVoteDb() {
        //VoteVM = Context.Votes
        //   .Where(v => v.User == User && v.Choice.Poll == Poll && (v.Type == VoteType.Yes || v.Type == VoteType.No || v.Type == VoteType.Maybe || v.Type == VoteType.Empty || v == null))
        //   .Select(v => new UserChoiceVoteViewModel(User, v.Choice, Poll))
        //   .ToList();

        VoteVM = _choicesSave
            .Select(c => new UserChoiceVoteViewModel(User, c, Poll))        // mettre la vue avec les choix
            .ToList(); ;

        // faire une fonction en db en utilisant la db
    }
    private void Save() {
        EditMode = false;
        //User.Votes = VoteVM.Where(u => u.Vote.Type != VoteType.Empty).Select(u => u.Vote).ToList();     // doit le faire sur les differents votes // <- pas bon ça récup tous les votes
        Context.SaveChanges();
        // On recrée la liste avec les nouvelles données
        OnRefreshData();
    }

    private void Cancel() {
        //https://stackoverflow.com/questions/5466677/undo-changes-in-entity-framework-entities
        //Context.Entry(Votes).Reload();

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
        EditMode = false;
        Dispose();
        RefreshVote();
    }

    private void Delete() {
        // Remove votes from the current poll
        var currentChoicesId = _choices.Select(c => c.Id).ToList();
        var voteToDelete = User.Votes.Where(v => currentChoicesId.Contains(v.Choice.Id)).ToList();

        foreach (var vote in voteToDelete) {
            User.Votes.Remove(vote);
        }
        // Save the changes and refresh the voting choices
        Context.SaveChanges();

        VoteVM.Clear(); 
        Context.SaveChanges();
        OnRefreshData();

    }

    protected override void OnRefreshData() {
        RefreshVote();
    }
}

