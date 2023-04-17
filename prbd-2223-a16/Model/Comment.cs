using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;

namespace MyPoll.Model;
public class Comment : EntityBase<MyPollContext> {

    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Creator))]
    public int UserId { get; set; }
    [ForeignKey(nameof(Poll))]
    public int PollId { get; set; }
    [Required]
    public string Text { get; set; }
    public virtual User Creator { get; set; }
    public virtual Poll Poll { get; set; }
    public DateTime Timestamp { get; set; }

    public Comment(int id, int userid, int pollId, string text, DateTime timestamp) {
        Id= id;
        UserId= userid;
        PollId= pollId;
        Text = text;
        Timestamp = timestamp;
    }

    public Comment() { }
}
