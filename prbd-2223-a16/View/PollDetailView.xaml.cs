using System.Windows.Controls;
using PRBD_Framework;
using MyPoll.Model;
using MyPoll.ViewModel;

namespace MyPoll.View;
public partial class PollDetailView : UserControlBase
{
    private readonly PollDetailViewModel _vm;
    public PollDetailView(Poll poll, bool isNew) {
        InitializeComponent();
        DataContext = _vm = new PollDetailViewModel(poll, isNew);
    }

}

