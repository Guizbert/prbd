using System.Windows;
using MyPoll.Model;
using MyPoll.ViewModel;
using PRBD_Framework;

namespace MyPoll; 

public partial class App : ApplicationBase<User, MyPollContext > {
    // TODO : afficher les best choices
    public enum Messages {
        MSG_NEW_MEMBER,             // nouveau membre
        MSG_CREATE_POLL,            // nouveau poll
        MSG_CREATED_POLL,           // création poll
        MSG_DISPLAY_POLL,           // afficher les polls
        MSG_CHOICE_POLL,            // afficher les polls
        MSG_DELETE_POLL,            // supprimer poll
        MSG_UPDATE_POLL,            // mettre à jour poll
        MSG_EDIT_POLL,              // pour aller sur la page edit de poll
        MSG_CLOSE_TABPOLLCREATED,
        MSG_POLL_NAMECHANGED,
        MSG_DELETE_CHOICE,
        MSG_NEWCHOICE_POLLSINGLE,

        MSG_CREATE_COMMENT,         // création commentaire
        MSG_DELETE_COMMENT,         // supprimer commentaire
        MSG_UPDATE_COMMENT,         // edit commentaire


        MSG_CREATE_VOTE,            //etc 
        MSG_DELETE_VOTE,
        MSG_UPDATE_VOTE,

        MSG_LOGIN,
        MSG_CLOSE_TAB,
        MSG_LOGOUT,
        MSG_REFRESH_DATA
    }
    protected override void OnStartup(StartupEventArgs e) {
        PrepareDatabase();
        Register<User>(this, Messages.MSG_LOGIN, user => {
            Login(user);
            NavigateTo<MainViewModel, User, MyPollContext>();
        });
        Register<User>(this, Messages.MSG_NEW_MEMBER, user => {
            //Login(user);
            NavigateTo<SignUpViewModel, User, MyPollContext>();
        });
        Register(this, Messages.MSG_LOGOUT, () => {
            Logout();
            NavigateTo<LoginViewModel, User, MyPollContext>();
        });
        Register<User>(this, Messages.MSG_NEW_MEMBER, user => {
            Login(user);
            NavigateTo<MainViewModel, User, MyPollContext>();
        });
        
    }

    private static void PrepareDatabase() {
        // Clear database and seed data
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();

        // Cold start
        Console.Write("Cold starting database... ");
        Context.Users.Find(0);
        Console.WriteLine("done");
    }

    protected override void OnRefreshData() {
        if (CurrentUser?.Email != null)
            CurrentUser = User.GetByEmail(CurrentUser.Email);
    }
}
