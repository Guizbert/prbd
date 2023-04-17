﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

using PRBD_Framework;

namespace MyPoll.Model;

public class Choice : EntityBase<MyPollContext> {

    [Key]
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
