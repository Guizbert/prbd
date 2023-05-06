using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using MyPoll.Model;
using MyPoll.View;
using PRBD_Framework;

namespace MyPoll.ViewModel;
public class LoginViewModel : ViewModelCommon {
    public ICommand LoginCommand { get; set; }

    public ICommand OpenSignUpCommand { get; }
    private string _email;
    private string _password;

    public string Email {
        get => _email;
        set => SetProperty(ref _email, value, () => Validate());
    }

    public string Password {
        get => _password;
        set => SetProperty(ref _password, value, () => Validate());
    }
    public LoginViewModel() : base() {
        LoginCommand = new RelayCommand(LoginAction,
            () => { return _email != null && _password != null && !HasErrors; });
       

        OpenSignUpCommand = new RelayCommand(OpenSignUp);
    }
    private void LoginAction() {
        if (Validate()) {
            var user = Context.Users.SingleOrDefault(user => user.Email == Email);
            if(user != null) {
                NotifyColleagues(App.Messages.MSG_LOGIN, user);
            }
        }
    }
    private void OpenSignUp() {
        NotifyColleagues(App.Messages.MSG_NEW_MEMBER, new User());
    }

    public override bool Validate() {
        ClearErrors();
        ValidateEmail();
        ValidatePassword();
        return !HasErrors;
    }


    public bool ValidateEmail() {
        var user = Context.Users.SingleOrDefault(user => user.Email == Email);
        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        Match match = regex.Match(Email);
        if (!match.Success) {
            AddError(nameof(Email), "email incorrect");
        }
        if (string.IsNullOrEmpty(Email))
            AddError(nameof(Email), "required");
        if (user == null) {
            AddError(nameof(Email), "n'existes pas ");
        }
        return !HasErrors;
    }
    public bool ValidatePassword() {
        var user = Context.Users.FirstOrDefault(user => user.Email == Email);

        if (string.IsNullOrEmpty(Password)) {
            AddError(nameof(Password), "required");
        } else if (user != null && !SecretHasher.Verify(Password,user.Password))
            AddError(nameof(Password), "mauvais password");

        return !HasErrors;
    }
    protected override void OnRefreshData() {
    }

    private void LoginAsUser_Click(object sender, RoutedEventArgs e) {
        Button button = sender as Button;
        switch (button.Tag.ToString()) {
            case "harry":
                LoginCommand.Execute("harry@test.com");
                break;
            case "john":
                LoginCommand.Execute("john@test.com");
                break;
            default:
                break;
        }
        LoginCommand.Execute(button);
    }

    private void LoginAsAdmin_Click(object sender, RoutedEventArgs e) {
        Email= "admin@test.com";
        Password = "admin";
        LoginCommand.Execute("admin@test.com");
        LoginAction();
    }
}

