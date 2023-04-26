using System.Windows;
using System.Windows.Controls;
using MyPoll.ViewModel;
using PRBD_Framework;

namespace MyPoll.View {
    public partial class LoginView : WindowBase {
        public LoginView() {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void LoginAsHarry(object sender, RoutedEventArgs e) {
            txtEmail.Text = "harry@test.com";
            txtPassword.Password = "harry";
            var viewModel = (LoginViewModel)DataContext;
            viewModel.LoginCommand.Execute(txtEmail);
        }

        private void LoginAsJohn(object sender, RoutedEventArgs e) {
            txtEmail.Text = "john@test.com";
            txtPassword.Password = "john";
            var viewModel = (LoginViewModel)DataContext;
            viewModel.LoginCommand.Execute(txtEmail);
        }

        private void LoginAsAdmin(object sender, RoutedEventArgs e) {
            txtEmail.Text = "admin@test.com";
            txtPassword.Password = "admin";
            var viewModel = (LoginViewModel)DataContext;
            viewModel.LoginCommand.Execute(txtEmail);
        }
    }
}
