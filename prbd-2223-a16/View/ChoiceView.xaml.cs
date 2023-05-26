using System.Windows.Controls;
using PRBD_Framework;
using MyPoll.Model;
using MyPoll.ViewModel;

namespace MyPoll.View; 
/// <summary>
/// Logique d'interaction pour ChoiceView.xaml
/// </summary>
public partial class ChoiceView : UserControlBase {

    private readonly ChoiceViewModel _vm;

    public ChoiceView(Poll poll) {
        InitializeComponent();
        DataContext = _vm = new ChoiceViewModel(poll);
    }
    public ChoiceView() {
        InitializeComponent();
    }
}
