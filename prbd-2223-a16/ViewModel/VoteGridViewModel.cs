using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class VoteGridViewModel : ViewModelCommon
{
    private Poll _poll;

    public Poll Poll {
        get => _poll;
        set => SetProperty(ref _poll, value);
    }
    public VoteGridViewModel(Poll poll)
    {
        _poll = poll;

        _choices = poll.Choices.OrderBy(c => c.Label).ToList();

        var participants = poll.Participants.OrderBy(p => p.FullName).ToList();

        _userVm = participants.Select(s => new UserChoiceViewModel(this, s, _choices)).ToList();
    }
    public VoteGridViewModel()
    {

    }
    private List<Choice> _choices;
    public List<Choice> Choices => _choices;

    private bool _editMode;
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value);
    }
    private List<UserChoiceViewModel> _userVm;
    public List<UserChoiceViewModel> UserVm => _userVm;

    // Méthode appelée lorsqu'on veut éditer une ligne ou lorsqu'on a fini d'éditer une ligne
    public void AskEditMode(bool editMode) {
        EditMode = editMode;

        // Change la visibilité des boutons de chacune des lignes
        // (voir la logique dans UserChoiceViewModel)
        foreach (var u in UserVm)
            u.Changes();
    }

    public void Cancel() {
        App.ClearContext();
        NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);

    }
}
