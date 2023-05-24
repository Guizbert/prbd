using System.Collections.ObjectModel;
using System.Windows;
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
    public bool IsCreator => CurrentUser == Poll.Creator || CurrentUser.Role == Role.Administrator;
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
        set => SetProperty(ref _addComment, value);
    }
    //Liste des commentaires :

    //public ICollection<Comment> Commentaire() => Poll.commentaires;

    private ObservableCollection<Comment> _commentaire;
    public ObservableCollection<Comment> Commentaire {
        get => _commentaire;
        set => SetProperty(ref _commentaire, value, () => AddCommentAction());
    }


    // si current User = personne qui a fait un commentaire alors afficher poubelle
    // si Creator afficher bouton edit et Delete
    public ICommand Edit { get; set; }
    public ICommand Delete { get; set; }
    public ICommand AddCommentCommand { get; set; }
    public ICommand DeleteCommentCommand { get; set; }
    public ICommand ShowTextBoxCommand { get; set; }

    private MessageBox _openDialogCommand ;
    public MessageBox DialogCommand {
        get => _openDialogCommand;
        set => SetProperty(ref _openDialogCommand, value);
    }

    private string _textToAdd;

    public string TextToAdd {
        get => _textToAdd;
        set {
            if (!string.Equals(_textToAdd, value)) {
                _textToAdd = value;
                RaisePropertyChanged();
            }
        }
    }
    public void ShowTextBox() {
        AddComment = true;
        Console.WriteLine(AddComment + " Can write ? <--------");
    }
    public void AddCommentAction() {
        if (string.IsNullOrEmpty(TextToAdd)) {
            // Le Text est vide ou null -> ne rien faire
            return;
        }
        if (Poll.commentaires.Any(c => c.Text == TextToAdd)) {
            return;
        }
        else{
            var comment = new Comment(CurrentUser, Poll, TextToAdd, DateTime.Now);
            Context.Comments.Add(comment);

            Commentaire.Add(comment);
            Poll.commentaires.Add(comment);
            TextToAdd = string.Empty;
            AddComment = false;
            Context.SaveChanges();
            refreshComm();
        }
        RaisePropertyChanged();

    }

    public void refreshComm() {
        
        Console.WriteLine(AddComment);
        foreach(var comm in Commentaire) {
            Console.WriteLine(comm.Text);
        }
        RaisePropertyChanged(nameof(Commentaire));
    }
    // editAction et CanDoAction


    public bool CanDoAction =>  CurrentUser == Poll.Creator || CurrentUser.Role == Role.Administrator;


    public bool CanDeleteComment(Comment comment) {
        return Commentaire.Where(c => c == comment && c.Creator == CurrentUser).Any();
    }


    // DeleteAction
    private void DeleteAction() {

        if (CanDoAction) {
            if(App.Confirm("Vous êtes sur le point de supprimer le poll '" + Poll.Name + "' cette action est irrévocable")) {
                Poll.Delete();
                NotifyColleagues(App.Messages.MSG_UPDATE_POLL, Poll);
                NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
            }
           
        }
        //CancelAction();
    }
    private void DeleteCommentAction(Comment comment) {
        Console.WriteLine(CanDeleteComment(comment));
       // var canDelete = Commentaire.Where(c => c == comment && c.Creator == CurrentUser || CurrentUser.Role == Role.Administrator).Any();

       // if (canDelete) {
            Commentaire.Remove(comment);
            Context.Remove(comment);
            Poll.commentaires.Remove(comment);
            Context.SaveChanges();  
       // }
              
    }


    public PollChoicesViewModel(Poll poll, bool isNew) {
        _voteGridViewModel = new VoteGridViewModel(poll);
        _poll = poll;
        if (Poll.commentaires == null) {
            Commentaire = new ObservableCollection<Comment>();
        }else {
            Commentaire = new ObservableCollection<Comment>(Poll.commentaires.OrderBy(c => c.Timestamp));
        }
        Edit = new RelayCommand( () => {
            PollDetailViewModel = new PollDetailView(poll, isNew);
            CanEditPoll = true;
        });
        Delete = new RelayCommand(DeleteAction);
        DeleteCommentCommand = new RelayCommand<Comment>(DeleteCommentAction);
        AddCommentCommand = new RelayCommand(AddCommentAction);
        ShowTextBoxCommand = new RelayCommand(ShowTextBox);

        Console.WriteLine(IsCreator + " Is Creator ? <--------");
        Console.WriteLine(AddComment + " I AddComment ? <--------");
    }


}

