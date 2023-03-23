using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PRBD_Framework;

namespace MyPoll.Model;

public class Choice : EntityBase<MyPollContext> {
    public int Id { get; set; }
    public int PollId { get; set; }
    public virtual Poll Poll { get; set; }
    public string Label { get; set; }
    public virtual ICollection<Vote> Votes { get; set; }

    public Choice(int id, int pollId, string label) {
        Id = id;
        PollId = pollId;
        Label = label;
    }

    public Choice() { }
}
