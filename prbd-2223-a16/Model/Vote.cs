using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;

namespace MyPoll.Model;

public enum VoteType {
    Yes, No, Maybe, Empty
}


public class Vote : EntityBase<MyPollContext>
{
    [ForeignKey(nameof(Choice))]
    public int ChoiceId { get; set; }
    public virtual Choice Choice { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public virtual User User { get; set; }
    public VoteType Type { get; set; }

    public Vote(int choiceId, int userId)
    {
        ChoiceId = choiceId;
        UserId = userId;
    }
    public Vote(Choice choice, User user)
    {
        ChoiceId = choice.Id;
        UserId = user.Id;
    }
    public Vote(Choice choice, User user, VoteType voteType)
    {
        ChoiceId = choice.Id;
        UserId = user.Id;
        Type = voteType;
    }

    public Vote() { }

    public double Value => Type switch {
        VoteType.Yes => 1,
        VoteType.Maybe => 0.5,
        VoteType.No => -1,
        VoteType.Empty => 0,
        _ => 0 // throw new Exception("bad vote value"),
    };
}
