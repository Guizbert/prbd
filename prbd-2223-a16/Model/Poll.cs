using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using PRBD_Framework;

namespace MyPoll.Model;

public enum PollType {
    Single, Multiple,
}

public class Poll : EntityBase<MyPollContext> {
    public int Id { get; set; }
    public string Name { get; set; }

    public PollType Type { get; set; }
    public int CreatorId { get; set; }

    public bool Closed { get; set; }
    public virtual ICollection<User> Participants { get; set; }
    public virtual ICollection<Participation> Participations{ get; set; }

    public virtual ICollection<Vote> Vote { get; set; }

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
}

