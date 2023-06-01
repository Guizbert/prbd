using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;
public class ChoiceListViewModel : ViewModelCommon {

    public ICommand EditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand DeleteCommand { get; }

    public event Action Changed;

     public ChoiceListViewModel(Poll p, Choice c ,ChoiceViewModel choiceVm)
     {
        Poll = p;
        _choice = c;
        _choiceVM = choiceVm;
        _label = c.Label;
        EditCommand = new RelayCommand(() => EditMode = true);
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
        DeleteCommand = new RelayCommand(Delete);

     }


    public ChoiceListViewModel(){}

    private Poll _poll;
    public Poll Poll {
        get => _poll;
        set => SetProperty(ref _poll, value);
    }

    private string _label;
    public string Label {
        get => _label;
        set => SetProperty(ref _label, value);
    }

    private Choice _choice;
    public Choice Choice {
        get => _choice;
        set=> SetProperty(ref _choice, value);
    }
    private bool _editMode;
    // La visbilité des boutons de sauvegarde et d'annulation sont bindés sur cette propriété
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value, EditModeChanged);
    }

    private void EditModeChanged() {
        ChoiceVM.EditMode = _editMode;

        Console.WriteLine(_editMode + " < - - - - test EditModeChanged");

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




    private void Save() {
        Console.WriteLine(_choiceVM.ChoicesVm.Any(c => c.Label == Label && c != this));
        if (Validate()) {
            Choice.Label = Label;
            EditMode = false;
            ClearErrors();
        }
    }

    public override bool Validate() {
        ClearErrors();
        ValidateLabel();
        return !HasErrors;
    }
    public bool ValidateLabel() {
        if (_choiceVM.ChoicesVm.Any(c => c.Label == Label && c != this)) {
            AddError(nameof(Label), "Already exist");
        } else if (Label.Length < 2)
            AddError(nameof(Label), "Too short");
        return !HasErrors;
    }

    private void Cancel() {
        Label = Choice.Label;
        EditMode = false;
        Choice.Reload();
        //Context.SaveChanges();
       // RefreshChoice();
        //Changed?.Invoke(); // déclenche l'event. le ? pour check si l'event est null
        Dispose();
        ClearErrors();
        RaisePropertyChanged(nameof(Label));
    }

    private void Delete() {
        if (EditMode) { EditMode= false; }
        if (Choice.Votes.Count > 0) {
            if(App.Confirm("You're about to delete the choice '"+Choice.Label+"' are you sure?")) {
                //Poll.Choices.Remove(Choice);
                Context.Choices.Remove(Choice);
                _choiceVM.ChoicesVm.Remove(this);
                Changed?.Invoke(); // déclenche l'event. le ? pour check si l'event est null
            }
        } else {
            //Poll.Choices.Remove(Choice);
            Context.Choices.Remove(Choice);
            _choiceVM.ChoicesVm.Remove(this);
            Changed?.Invoke(); // déclenche l'event. le ? pour check si l'event est null
        }
    }

   
    protected override void OnRefreshData() {
        //RefreshChoice();
    }

}

