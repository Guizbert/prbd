using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

internal class PollDetailViewModel : ViewModelCommon {

    public ICommand Save { get; set; }
    public ICommand Cancel { get; set; }
    public ICommand Delete { get; set; }

    //private User _user;
    //public User User {
    //    get { return _user; }
    //    set => SetProperty(ref _user, value);
    //}

    // check si c'est un new poll

    private Poll _poll;
    public Poll Poll {
        get => _poll;
        set => SetProperty(ref _poll, value);
    }
    private bool _isNew;
    public bool IsNew {
        get => _isNew;
        set => SetProperty(ref _isNew, value);
    }
    public string Title {
        get => Poll.Name;
        set => SetProperty(Poll.Name, value, Poll, (t, v) => {
            t.Name = v;
            NotifyColleagues(App.Messages.MSG_UPDATE_POLL, Poll);
        });
    }

    public string Creator {
        get => $"(Created by {CurrentUser.FullName})";
    }
    private bool _isClosed;
    public bool IsClosed {
        get => _isNew;
        set => SetProperty(ref _isClosed, value);
    }


    public IEnumerable<User> AllParticipants {
        get {
            return Poll.AllUsers;
        }
    }
    public static PollType[] getTypevalues => Poll.getTypes();

    private PollType _selectedPollType;

    public PollType SelectedPollType {
        get => _selectedPollType;
        set {
            _selectedPollType = value;
            RaisePropertyChanged(nameof(SelectedPollType));
        }
    }

    private string _selectedUser;

    public string SelectedUser {
        get => _selectedUser;
        set {
            _selectedUser = value;
            RaisePropertyChanged(nameof(SelectedUser));
        }
    }



    public PollDetailViewModel(Poll poll, bool isNew)
    {
        Poll = poll;
        IsNew= isNew;
        Save = new RelayCommand(SaveAction, CanSaveAction);
        Cancel = new RelayCommand(CancelAction, CanCancelAction);
        Delete = new RelayCommand(DeleteAction, () => !IsNew);

        RaisePropertyChanged();

    }

    public override void SaveAction()
    {
        if (IsNew) {
            Context.Add(Poll);
            IsNew = false;
        }
        Context.SaveChanges();
        NotifyColleagues(App.Messages.MSG_UPDATE_POLL, Poll);

    }

    private bool CanSaveAction() {
        if (IsNew) 
            return !string.IsNullOrEmpty(Title);
        return Poll != null && Poll.IsModified;

    }

    public override void CancelAction() {
        if (IsNew) {
            IsNew = false;
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
        } else {
            Poll.Reload();
            RaisePropertyChanged();
        }
    }
    private bool CanCancelAction() {
        return Poll != null && (IsNew || Poll.IsModified);
    }
    private void DeleteAction() {
        CancelAction();
        Poll.Delete();
        NotifyColleagues(App.Messages.MSG_UPDATE_POLL, Poll);
        NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
    }

}

