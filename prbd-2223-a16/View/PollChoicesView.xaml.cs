using System.Windows.Controls;
using PRBD_Framework;
using MyPoll.Model;
using MyPoll.ViewModel;

namespace MyPoll.View;
public partial class PollChoicesView : UserControlBase {
    private readonly PollChoicesViewModel _vm;
    public PollChoicesView(Poll poll) {
        InitializeComponent();
        DataContext = _vm = new PollChoicesViewModel(poll);
    }
}

