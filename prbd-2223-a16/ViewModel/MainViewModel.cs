using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class MainViewModel : ViewModelCommon {

    public ICommand ReloadDataCommand { get; set; }

    //public string Title { get; } = "prbd-2223-a16";
    public static string Title {
        get => $"{CurrentUser.FullName}'s polls";
        //get => $"My Social Network ({CurrentUser?.Pseudo})";
    }
}
