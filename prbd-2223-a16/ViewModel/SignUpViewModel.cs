using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using MyPoll.View;
using PRBD_Framework;

namespace MyPoll.ViewModel;
class SignUpViewModel : ViewModelCommon {
    /*pseudo, mail , password, passwordConfirm*/

    public ICommand SignUpCommand { get; set; }

    private string _pseudo;
    private string _email;
    private string _password;
    private string _passwordConfirm;

    public string Pseudo {
        get => _pseudo;
        set => SetProperty(ref _pseudo, value, () => Validate());
    }
    public string Email {
        get => _email;
        set => SetProperty(ref _email, value, () => Validate());
    }

    public string Password {
        get => _password;
        set => SetProperty(ref _password, value, () => Validate());
    }
    public string PasswordConfirm {
        get => _passwordConfirm;
        set => SetProperty(ref _passwordConfirm, value, () => Validate());
    }

    public SignUpViewModel() : base() {
        SignUpCommand = new RelayCommand(SignUpAction, () => {
            return _pseudo != null && _email != null && _password != null && _passwordConfirm != null;
            });
        
    }
    private void SignUpAction() {
        if (Validate()) {
            var newUser = new User(
                Pseudo = Pseudo,
                Email = Email,
                Password = SecretHasher.Hash(Password));
            Context.Users.Add(newUser);
            Context.SaveChanges();
            var signUpWindow = App.Current.Windows.OfType<SignUpView>().FirstOrDefault();
            
            NotifyColleagues(App.Messages.MSG_NEW_MEMBER, newUser);
            if (signUpWindow != null) {
                signUpWindow.Close();
            }
        }
    }

    public override bool Validate() {
        ClearErrors();
        validatePseudo();
        validateEmail();
        validatePassword();
        validatePasswordConfirm();
        return !HasErrors;
    }

    private bool validatePseudo() {
        if(string.IsNullOrEmpty(Pseudo)) {
            AddError(nameof(Pseudo), "required pseudo");
        }
        if(Pseudo.Length < 2) {
            AddError(nameof(Pseudo), "2 characters minimum");
        }
        return !HasErrors;
    }
    private bool validateEmail() {
        var user = Context.Users.SingleOrDefault(user => user.Email == Email);
        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        Match match = regex.Match(Email);
        if (!match.Success) {
            AddError(nameof(Email), "email incorrect");
        }
        if (string.IsNullOrEmpty(Email)) {
            AddError(nameof(Email), "required");
        } else if (user != null && user.Email == Email) {
            AddError(nameof(Email), "Already exist");
        }

        return !HasErrors;
    }

    private bool validatePassword() {
        if (string.IsNullOrEmpty(Password)) {
            AddError(nameof(Password), "required");
        }
        if(Password.Length < 3) {
            AddError(nameof(Password), "too short");
        }

        return !HasErrors;
    }
    private bool validatePasswordConfirm() {
        if (string.IsNullOrEmpty(Password)) {
            AddError(nameof(Password), "required");
        }
        if (validatePassword()) {
            if(Password != PasswordConfirm) {
                AddError(nameof(PasswordConfirm), "Same password required");
            }
        }
        return !HasErrors;

    }
}

