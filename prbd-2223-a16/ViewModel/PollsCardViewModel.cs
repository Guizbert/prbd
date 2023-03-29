using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyPoll.Model;

namespace MyPoll.ViewModel;
public class PollsCardViewModel : ViewModelCommon
{
    private readonly Poll _poll;

    public Poll Poll
    {
        get => _poll;
        private init => SetProperty(ref _poll, value);
    }

    public string name => Poll.Name;

    public PollType Type => Poll.Type;


    //binder dans la vue avec le creator.name


    /*

      public string Name { get; set; }

public PollType Type { get; set; }


public int CreatorId { get; set; }
public bool Closed { get; set; }
public virtual ICollection<User> Participants { get; set; }
public virtual ICollection<Choice> Choices { get; set; }

public virtual ICollection<Comment> commentaires { get; set; }

public Poll( int id, string na*/
}

