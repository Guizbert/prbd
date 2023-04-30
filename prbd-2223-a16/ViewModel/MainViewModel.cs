using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class MainViewModel : ViewModelCommon {

    public ICommand ReloadDataCommand { get; set; }

    public MainViewModel() : base() {
        ReloadDataCommand = new RelayCommand(() => {
            // refuser un reload s'il y a des changements en cours
            if (Context.ChangeTracker.HasChanges()) return;
            // permet de renouveller le contexte
            App.ClearContext();
            // notifie qu'il faut rafraîchir les données
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
        });
    }


    //public string Title { get; } = "prbd-2223-a16";
    public static string Title {
        get => $"{CurrentUser.FullName}'s polls";
        //get => $"My Social Network ({CurrentUser?.Pseudo})";
    }
}
