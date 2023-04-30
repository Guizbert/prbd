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


    [Required, ForeignKey(nameof(Poll))]
    public int PollId { get; set; }
    public virtual Poll Poll { get; set; }

    [Required, ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public virtual User User { get; set; }


    public Participation( int  pollid, int userid) {
        PollId = pollid;
        UserId = userid;
    }
    public Participation( ) {
    }


}

