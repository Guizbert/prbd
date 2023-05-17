using System.Windows.Controls;
using System.Windows.Input;
using MyPoll.Model;
using MyPoll.View;
using MyPoll.ViewModel;
using Newtonsoft.Json.Bson;
using PRBD_Framework;

namespace MyPoll;
internal class PollChoicesViewModel : ViewModelCommon{

    private Poll _poll;
    public Poll Poll {
        get => _poll;
        set => SetProperty(ref _poll, value);

    }
    private VoteGridViewModel _voteGridViewModel;

    public VoteGridViewModel VoteGridViewModel => _voteGridViewModel;

    private UserControl _editPoll;
    public UserControl PollDetailViewModel {
        get => _editPoll;
        set => SetProperty(ref _editPoll, value);
    }
    private bool _canEditPoll;
    public bool CanEditPoll {
        get => _canEditPoll;
        set=> SetProperty(ref _canEditPoll, value);
    }
    public bool IsCreator => CurrentUser == Poll.Creator;
    public bool CanReoPen => CurrentUser == Poll.Creator && Poll.Closed;

    // afficher en haut de la page:
    public string Title { get => Poll.Name; }
    public string Creator => Poll.Creator.FullName;
        // afficher message si poll est closed (Que le creator / Admin?) qui peut y avoir accès avec un background rose un peu
    public bool IsClosed => Poll.Closed;

    // Commentaire pour le 'bouton add comment'

    private bool _addComment;
    public bool AddComment {
        get => _addComment;
        set => SetProperty(ref _addComment, value, () => ShowTextBox());
    }
    public bool CanAddComment => AddComment == true;
    //Liste des commentaires :
    // si current User = personne qui a fait un commentaire alors afficher poubelle

    public ICollection<Comment> Commentaire() => Poll.commentaires;


    // si Creator afficher bouton edit et Delete
    public ICommand Edit { get; set; }
    public ICommand Delete { get; set; }
    public ICommand AddCommentCommand { get; set; }
    public ICommand ShowTextBoxCommand { get; set; }


    private string _textToAdd;

    public string TextToAdd {
        get => _textToAdd;
        set => SetProperty(ref _textToAdd, value, () => AddCommentAction());
    }
    public void ShowTextBox() {
        if (!AddComment)
            AddComment = !AddComment;
        Console.WriteLine(AddComment + " Can write ? <--------");

    }
    public void AddCommentAction() {
        if(TextToAdd != null) {
            var comment = new Comment(CurrentUser, Poll, TextToAdd, DateTime.Now);
            Poll.commentaires.Add(comment);
            Context.Add(comment);
            Context.SaveChanges();
            AddComment= false;
        }
    }
    // editAction et CanDoAction


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


    public PollChoicesViewModel(Poll poll, bool isNew) {
        _voteGridViewModel = new VoteGridViewModel(poll);
        _poll = poll;
        Console.WriteLine(CanEditPoll);

        Edit = new RelayCommand( () => {
            PollDetailViewModel = new PollDetailView(poll, isNew);
            CanEditPoll = true;
        });
        Delete = new RelayCommand(DeleteAction, CanDoAction);
        AddCommentCommand = new RelayCommand(AddCommentAction);
        ShowTextBoxCommand = new RelayCommand(ShowTextBox);

        Console.WriteLine(IsCreator + " Is Creator ? <--------");
        Console.WriteLine(AddComment + " I AddComment ? <--------");
    }


}

