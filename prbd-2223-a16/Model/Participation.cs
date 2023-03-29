using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;

namespace MyPoll.Model;
public class Participation : EntityBase<MyPollContext> {


    [ForeignKey(nameof(Poll))]
    public int PollId { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    public virtual Poll Poll { get; set; }
    public virtual User User { get; set; }


}

