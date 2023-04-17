using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;

namespace MyPoll.Model;

public enum PollType {
     Multiple, Single
}

public class Poll : EntityBase<MyPollContext> {

    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public PollType Type { get; set; }

    [Required ,ForeignKey(nameof(Creator))]
    public int CreatorId { get; set; }

    public virtual User Creator { get; set; }
    public bool Closed { get; set; }

    public string BestChoice => getBestChoice(this);

    public virtual ICollection<User> Participants { get; set; }
    public virtual ICollection<Choice> Choices { get; set; }

    public virtual ICollection<Comment> commentaires { get; set; }

    public Poll( int id, string name, int creatorId, PollType type) {
        Id = id;
        Name = name;
        CreatorId = creatorId;
        Type = type;
    }
    public Poll( int id, string name, int creatorId, bool isClosed) {
        Id = id;
        Name = name;
        CreatorId = creatorId;
        Closed = isClosed;
    }

    public Poll() { }


    private string getBestChoice(Poll poll) {
        var bestChoice = from c in Context.Choices
                             // faire un where pour check le tricount
                         where c.PollId == poll.Id
                         orderby c.Votes descending
                         select c.Label;
        var bestc = bestChoice.FirstOrDefault();
        return bestc;
    }
}

