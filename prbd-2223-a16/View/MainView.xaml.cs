using PRBD_Framework;
using MyPoll.Model;
using System.Windows.Controls;

namespace MyPoll.View;

public partial class MainView : WindowBase {
    public MainView() {
        InitializeComponent();
        Console.WriteLine(polls);

        Register<Poll>(App.Messages.MSG_CREATE_POLL,
            poll => DoDisplayPoll(poll, true));
        
        Register<Poll>(App.Messages.MSG_CREATED_POLL,             // CLOSE TAB après creation poll
            poll => DoCloseTab(poll));
      
        Register<Poll>(App.Messages.MSG_CHOICE_POLL,              // si on veut faire des votes, etc
            poll => DoDisplayPollElements(poll, false));


        Register<Poll>(App.Messages.MSG_CLOSE_TAB,              // si on veut faire des votes, etc
            poll => DoCloseTab(poll));


    }


    private void DoDisplayPoll(Poll poll, bool isNew) {
            OpenTab(isNew ? "<New Poll>" : poll.Name, poll.Name,() => new PollDetailView(poll, isNew));
    }
    private void DoDisplayPollElements(Poll poll, bool isNew) {
        if(poll != null) {
            OpenTab(poll.Name, poll.Name, () => new PollChoicesView(poll, isNew));
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
