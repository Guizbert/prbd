using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
    public ChoiceViewModel(Poll poll) {
       // _choices = new ObservableCollectionFast<Choice>(poll.Choices.OrderBy(c => c.Label).ToList());
        _choices = new ObservableCollectionFast<Choice>(poll.Choices.OrderBy(c => c.Label));
        Poll = poll;

        _choicesVm = new ObservableCollectionFast<ChoiceListViewModel>(_choices.Select(c =>  new ChoiceListViewModel(poll, c, this)));
        //_choicesVm = new ObservableCollectionFast<ChoiceListViewModel>(poll, c, this);

        AddChoiceCommand = new RelayCommand(AddChoice);

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
        // ajouter validation (if(!haserror)
        var newChoice = new Choice { Label = ChoiceLabel };
        // faire validation
        Poll.Choices.Add(newChoice);
        _choices.Add(newChoice);

        //ajout event 
        var AddToVm = new ChoiceListViewModel(Poll, newChoice, this);
        AddToVm.Changed += () => {
            Console.WriteLine("test changed event ");
            ChoicesVm.Remove(AddToVm);
        };
        _choicesVm.Insert(0, AddToVm);
        ChoiceLabel = "";
        Context.Choices.Add(newChoice);
        RaisePropertyChanged();
        NotifyColleagues(App.Messages.MSG_UPDATE_COMMENT, Poll);
        
    }
    public void EditChoice() {


        // ajouter validation (if(!haserror)
        var newChoice = new Choice { Label = ChoiceLabel };
        // faire validation
        Poll.Choices.Add(newChoice);
        _choices.Add(newChoice);

        //ajout event 
        var AddToVm = new ChoiceListViewModel(Poll, newChoice, this);
        AddToVm.Changed += () => {
            Console.WriteLine("test");
            ChoicesVm.Remove(AddToVm);
        };
        _choicesVm.Insert(0, AddToVm);
        ChoiceLabel = "";
        Context.Choices.Add(newChoice);
        RaisePropertyChanged();
        NotifyColleagues(App.Messages.MSG_UPDATE_COMMENT, Poll);
        
    }
    


    public void Cancel() {
        App.ClearContext();
        NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
    }


}

