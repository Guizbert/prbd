using System.Windows.Input;
using MyPoll.Model;
using MyPoll.ViewModel;

using PRBD_Framework;

namespace MyPoll;
internal class PollChoicesViewModel : ViewModelCommon{

    private Poll _poll;
    public Poll Poll {
        get => _poll;
        set => SetProperty(ref _poll, value);

    }

    // afficher en haut de la page:
    public string Title { get => Poll.Name; }
    public string Creator => Poll.Creator.FullName;
        // afficher message si poll est closed (Que le creator / Admin?) qui peut y avoir accès avec un background rose un peu
    public bool IsClosed => Poll.Closed;

    // Commentaire pour le 'bouton add comment'

    private bool _addComment;
    public bool AddComment {
        get => _addComment;
        set => SetProperty(ref _addComment, value, () => AddCommentAction());
    }
    //Liste des commentaires :
    // si current User = personne qui a fait un commentaire alors afficher poubelle

    public ICollection<Comment> Commentaire() => Poll.commentaires;


    // si Creator afficher bouton edit et Delete
    public ICommand Edit { get; set; }
    public ICommand Delete { get; set; }
    public ICommand AddCommentCommand { get; set; }


    public void AddCommentAction() {
        if(!_addComment)
            _addComment = true;
    //   showTextBox();
    }


    // editAction et CanDoAction

    public void EditAction() {
        if (CanDoAction()) {
            NotifyColleagues(App.Messages.MSG_EDIT_POLL, Poll);
        }
    }


    public bool CanDoAction() {
        return CurrentUser == Poll.Creator || CurrentUser.Role == Role.Administrator;
    }


    // DeleteAction
    private void DeleteAction() {
        if (CanDoAction()) {
            Poll.Delete();
            NotifyColleagues(App.Messages.MSG_UPDATE_POLL, Poll);
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
        }
        //CancelAction();
    }


    public PollChoicesViewModel(Poll poll) { 
        Poll = poll;
        Edit = new RelayCommand(EditAction, CanDoAction);
        Delete = new RelayCommand(DeleteAction, CanDoAction);
        AddCommentCommand = new RelayCommand(AddCommentAction);

    }

}

