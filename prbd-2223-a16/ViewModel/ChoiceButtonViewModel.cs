using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class ChoiceButtonViewModel : ViewModelCommon {

    public ChoiceButtonViewModel(ChoiceViewModel choicevm, Choice choice) {
        _choice = choice;
        Poll = choice.Poll;
        User = choice.Poll.Creator;

        RefreshChoice();

        Console.WriteLine("CREATION DE BTN VM");
        EditCommand = new RelayCommand(() => EditMode = true);
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
        DeleteCommand = new RelayCommand(Delete);
    }

    public Poll Poll { get; set; }
    public ICommand EditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand DeleteCommand { get; }

    private Choice _choice;

    public User User { get; }

    private bool _editMode;


    // La visbilité des boutons de sauvegarde et d'annulation sont bindés sur cette propriété
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value, EditModeChanged);
    }

    private void EditModeChanged() {
        ChoiceVM.EditMode = _editMode;

        Console.WriteLine(_editMode+ "sdpfodspfokspdofkpsodfk 123 123 123 123");
        //_voteGridViewModel.AskEditMode(EditMode);
        ChoiceVM.AskEditMode(EditMode);
    }

    // Méthode appelée par le VM parent pour que la ligne mettre à jour la visibilité des boutons
    public void Changes() {
        RaisePropertyChanged(nameof(Editable));
    }


    // La ligne est éditable si elle n'est pas déjà en mode édition et si aucune autre ligne ne l'est
    // On récupére cette info via ParentEditMode
    // La visbilité des boutons d'édition et de suppression sont bindés sur cette propriété
    public bool Editable => !EditMode && !ParentEditMode;

    public bool ParentEditMode => _choiceVM.EditMode;


    private ChoiceViewModel _choiceVM = new();
    public ChoiceViewModel ChoiceVM {
        get => _choiceVM;
        private set => SetProperty(ref _choiceVM, value);
    }

    

    private void RefreshChoice() {
        _choiceVM.Choice = _choice;

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
        
        EditMode = false;
        Dispose();
        // On recrée la liste avec les nouvelles données
        RefreshChoice();
    }

    private void Delete() {
        

    }

    protected override void OnRefreshData() {
        RefreshChoice();
    }
}

