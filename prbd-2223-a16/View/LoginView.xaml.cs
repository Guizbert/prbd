using System.Windows;
using System.Windows.Controls;
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
        }

        private void LoginAsJohn(object sender, RoutedEventArgs e) {
            txtEmail.Text = "john@test.com";
            txtPassword.Password = "john";
        }

        private void LoginAsAdmin(object sender, RoutedEventArgs e) {
            txtEmail.Text = "admin@test.com";
            txtPassword.Password = "admin";
        }
    }
}
