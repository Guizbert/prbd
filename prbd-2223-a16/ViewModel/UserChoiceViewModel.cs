using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class UserChoiceViewModel : ViewModelCommon
{

    public UserChoiceViewModel(VoteGridViewModel voteGridViewModel, User user, List<Choice> choices) {
        _voteGridViewModel = voteGridViewModel;

        _choices = choices;

        User = user;
        RefreshVote();

        EditCommand = new RelayCommand(() => EditMode = true);
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
        DeleteCommand = new RelayCommand(Delete);
    }

    private VoteGridViewModel _voteGridViewModel;
    public ICommand EditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand DeleteCommand { get; }

    private List<Choice> _choices;

    public User User { get; }

    private bool _editMode;

    // La visbilité des boutons de sauvegarde et d'annulation sont bindés sur cette propriété
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value, EditModeChanged);
    }

    private void EditModeChanged() {
        // Lorsqu'on change le mode d'édition de la ligne, on le signale à chaque cellule
        foreach (UserChoiceVoteViewModel regVM in _voteVM) {
            regVM.EditMode = EditMode;
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
        // On crée, pour chaque inscription de l'étudiant, un RegistrationStudentCourseViewModel
        // qui sera utilisé par le RegistrationStudentCourseView
        // RegistrationsVM est la liste qui servira de source pour la balise <ItemsControl>
        VoteVM = _choices
            .Select(c => new UserChoiceVoteViewModel(User, c))        // mettre la vue avec les choix
            .ToList();
    }

    private void Save() {
        EditMode = false;
        
        User.Votes = VoteVM.Where(u => u.HasVotedYes).Select(u => u.Vote).ToList();     // doit le faire sur les differents votes
        Context.SaveChanges();
        // On recrée la liste avec les nouvelles données
        RefreshVote();
    }


    private void Cancel() {
        EditMode = false;
        // On recrée la liste avec les nouvelles données
        RefreshVote();
    }

    private void Delete() {
        User.Votes.Clear();
        Context.SaveChanges();
        // On recrée la liste avec les nouvelles données
        RefreshVote();
    }
}

