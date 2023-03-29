using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;

namespace MyPoll.Model;

public enum VoteType {
    Yes = 1, No = -1, Maybe = 1/2,
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

    public Vote() { }
}
