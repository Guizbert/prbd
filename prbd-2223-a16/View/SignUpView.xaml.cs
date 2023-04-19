using System.Windows;
using PRBD_Framework;

namespace MyPoll.View;

public partial class SignUpView : WindowBase {
    public SignUpView() {
        InitializeComponent();
    }
    private void btnCancel_Click(object sender, RoutedEventArgs e) {
        Close();
    }
}

