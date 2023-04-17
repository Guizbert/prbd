using System.Windows;
using MyPoll.Model;
using MyPoll.ViewModel;
using PRBD_Framework;

namespace MyPoll; 

public partial class App : ApplicationBase<User, MyPollContext > {

    public enum Messages {
        MSG_NEW_MEMBER,
        MSG_CREATE_POLL,
        MSG_DELETE_POLL,
        MSG_UPDATE_POLL,
        MSG_CREATE_COMMENT,
        MSG_DELETE_COMMENT,
        MSG_UPDATE_COMMENT,
        MSG_CREATE_VOTE,
        MSG_DELETE_VOTE,
        MSG_UPDATE_VOTE,
        MSG_CLOSE_TAB,
        MSG_LOGIN
    }
    protected override void OnStartup(StartupEventArgs e) {
        PrepareDatabase();
        Register<User>(this, Messages.MSG_LOGIN, user => {
            Login(user);
            NavigateTo<MainViewModel, User, MyPollContext>();
        });
        Register<User>(this, Messages.MSG_NEW_MEMBER, user => {
            Login(user);
            NavigateTo<MainViewModel, User, MyPollContext>();
        });
    }

    private static void PrepareDatabase() {
        // Clear database and seed data
        //Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();

        // Cold start
        Console.Write("Cold starting database... ");
        Context.Users.Find(0);
        Console.WriteLine("done");
    }
}
