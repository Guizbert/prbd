using PRBD_Framework;
using MyPoll.Model;
using System.Windows.Controls;

namespace MyPoll.View;

public partial class MainView : WindowBase {
    public MainView() {
        InitializeComponent();

        Register<Poll>(App.Messages.MSG_CREATE_POLL,
            poll => DoDisplayPoll(poll, true));

        Register<Poll>(App.Messages.MSG_DISPLAY_POLL,
            poll => DoDisplayPoll(poll, false));


    }


    private void DoDisplayPoll(Poll poll, bool isNew) {
        if(poll != null) {
            OpenTab(isNew ? "<New Poll>" : poll.Name, poll.Name,() => new PollDetailView(poll, isNew));
        }
    }


    private void OpenTab(string header, string tag, Func<UserControlBase> createView) {
        var tab = tabControl.FindByTag(tag);
        if (tab == null) 
            tabControl.Add(createView(), header, tag);
        else
            tabControl.SetFocus(tab);
    }

    private void DoRenameTab(string header) {
        if(tabControl.SelectedItem is TabItem tab) {
            MyTabControl.RenameTab(tab, header);
            tab.Tag = header;
        }
    }
    private void MenuLogout_Click(object sender, System.Windows.RoutedEventArgs e) {
        NotifyColleagues(App.Messages.MSG_LOGOUT);
    }

    private void DoCloseTab(Poll poll) {
        tabControl.CloseByTag(string.IsNullOrEmpty(poll.Name) ? "<New Poll>" : poll.Name);
    }

}
