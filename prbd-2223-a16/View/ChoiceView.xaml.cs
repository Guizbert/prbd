using System.Windows.Controls;
using PRBD_Framework;
using MyPoll.Model;
using MyPoll.ViewModel;

namespace MyPoll.View; 
/// <summary>
/// Logique d'interaction pour ChoiceView.xaml
/// </summary>
public partial class ChoiceView : UserControl {

    private readonly ChoiceViewModel _vm;

    public ChoiceView(Choice c ) {
        InitializeComponent();
        DataContext = _vm = new ChoiceViewModel(c);
    }
}
