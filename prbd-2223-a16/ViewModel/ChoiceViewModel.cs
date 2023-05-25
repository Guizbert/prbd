using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;
public class ChoiceViewModel : ViewModelCommon {
    private Poll _poll;

    public Poll Poll {
        get => _poll;
        set => SetProperty(ref _poll, value);
    }
    public ChoiceViewModel(Choice choice) {
        _choice = choice;

        Poll = choice.Poll;

        _choiceVm = new ChoiceButtonViewModel(this, choice);

        Console.WriteLine(_choiceVm);

        _label = choice.Label;
        Console.WriteLine(EditMode);
    }
    private Choice _choice;
    public Choice Choice {
        get => _choice;
        set => SetProperty(ref _choice, value);
    }

    public ChoiceViewModel(){}

    private string _label;
    public string Label {
        get => _label;
        set => SetProperty(ref _label, value);
    }

    private bool _editMode;
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value);
    }

    private ChoiceButtonViewModel _choiceVm;
    public ChoiceButtonViewModel ChoiceVm => _choiceVm;

    // Méthode appelée lorsqu'on veut éditer une ligne ou lorsqu'on a fini d'éditer une ligne
    public void AskEditMode(bool editMode) {
        EditMode = editMode;

        // Change la visibilité des boutons de chacune des lignes
        // (voir la logique dans UserChoiceViewModel)
        ChoiceVm.Changes();
    }

    public void Cancel() {
        App.ClearContext();
        NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);

    }

}

