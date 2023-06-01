using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyPoll.Model;
using PRBD_Framework;
using static MyPoll.App;

namespace MyPoll.ViewModel;
public class ChoiceViewModel : ViewModelCommon {
    private Poll _poll;

    public Poll Poll {
        get => _poll;
        set => SetProperty(ref _poll, value);
    }

    public ICommand AddChoiceCommand { get; set; }

    /* ==================================== CONSTRUCTEUR ====================================*/
    public ChoiceViewModel(Poll poll) {
        _choices = new ObservableCollectionFast<Choice>(poll.Choices.OrderBy(c => c.Label));
        Poll = poll;
        HasChoices();
        _choicesVm = new ObservableCollectionFast<ChoiceListViewModel>(_choices.Select(c =>  new ChoiceListViewModel(poll, c, this)));

        AddChoiceCommand = new RelayCommand(AddChoice);
    }

    private bool _noChoices;
    public bool NoChoices {
        get => _noChoices;
        set => SetProperty(ref _noChoices, value);
    }

    private string _choiceLabel;
    public string ChoiceLabel {
        get => _choiceLabel;
        set=> SetProperty(ref _choiceLabel, value);
    }
    public ChoiceViewModel() { }


    // choicesVm (va afficher les choices + boutons)
    private ObservableCollectionFast<ChoiceListViewModel> _choicesVm;
    public ObservableCollectionFast<ChoiceListViewModel> ChoicesVm => _choicesVm;  


    private ObservableCollectionFast<Choice> _choices;
    public ObservableCollectionFast<Choice> Choices {
        get => _choices;
        set => SetProperty(ref _choices, value);
    }

    private bool _editMode;
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value);
    }

    // Méthode appelée lorsqu'on veut éditer une ligne ou lorsqu'on a fini d'éditer une ligne
    public void AskEditMode(bool editMode) {
        EditMode = editMode;

        // Change la visibilité des boutons de chacune des lignes
        // (voir la logique dans UserChoiceViewModel)
    }
    public void AddChoice() {
        if (validate()) {
            // ajouter validation (if(!haserror)
            var newChoice = new Choice { Label = ChoiceLabel };
            // faire validation
            Poll.Choices.Add(newChoice);
            _choices.Add(newChoice);

            //ajout event 
            var AddToVm = new ChoiceListViewModel(Poll, newChoice, this);

            _choicesVm.Insert(0, AddToVm);
            ChoiceLabel = "";
            Context.Choices.Add(newChoice);

            HasChoices();
            RaisePropertyChanged();
            NotifyColleagues(App.Messages.MSG_UPDATE_POLL, Poll);
        }
    }

    public void EditChoice() {
        if (validate()) {
            var newChoice = new Choice { Label = ChoiceLabel };
            Poll.Choices.Add(newChoice);
            _choices.Add(newChoice);

            //ajout event 
            var AddToVm = new ChoiceListViewModel(Poll, newChoice, this);

            // abonnement a l'event
            AddToVm.Changed += () => {
                Console.WriteLine("test");
                ChoicesVm.Remove(AddToVm);
            };

            _choicesVm.Insert(0, AddToVm);
            ChoiceLabel = "";
            Context.Choices.Update(newChoice);
            RaisePropertyChanged();
            NotifyColleagues(App.Messages.MSG_UPDATE_COMMENT, Poll);
            NotifyColleagues(App.Messages.MSG_UPDATE_POLL, Poll);
        }
    }

    public void cancel() {
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
        NotifyColleagues(App.Messages.MSG_UPDATE_COMMENT, Poll);
        NotifyColleagues(App.Messages.MSG_UPDATE_POLL, Poll);

    }

    // PARTIE VALIDATION
    public void HasChoices() {
        NoChoices = _choicesVm.IsNullOrEmpty();
        RaisePropertyChanged(nameof(NoChoices));
    }
    public bool validate() {
        ClearErrors();
        ValidateLabel();
        return !HasErrors;

    }

    public bool ValidateLabel() {
        if(string.IsNullOrEmpty(ChoiceLabel)) {
            AddError(nameof(ChoiceLabel), "Required");
        }
        if (ChoiceLabel.Length < 2) {
            AddError(nameof(ChoiceLabel), "Too short");
        }
        if (Poll.Choices.Any(c => c.Label == ChoiceLabel) || _choices.Any(c => c.Label == ChoiceLabel)) {
            ClearErrors();
            AddError(nameof(ChoiceLabel),"already exist");
        }
        return !HasErrors;
    }


}

