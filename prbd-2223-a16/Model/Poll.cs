using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

    //public string BestChoice => getBestChoice(this);

    //public static IEnumerable<User> UserNotParticipating => UsersNotParticipating;

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
    }public Poll( string name, int creatorId, PollType type,  bool isClosed) {
        Name = name;
        CreatorId = creatorId;
        Type= type;
        Closed = isClosed;
    }

    public Poll() { }


    [NotMapped]
    public IEnumerable<Choice> BestChoice {
        get {
            if (Choices.Count == 0) return new List<Choice>();
            var maxScore = Choices.Select(c => c.Votes.Sum(v => v.Value)).Max();
            if (maxScore == 0) return new List<Choice>();
            var choices = Choices.Where(c => c.Votes.Sum(v => v.Value) == maxScore).ToList();

            //var choices = Context.Choices
            //.Include(c => c.Votes)
            //.Where(c => c.PollId == poll.Id)
            //.OrderByDescending(c => c.Votes.Count())
            //.ToList();

            //return choices.FirstOrDefault()?.Label;
            return choices;
        }
    }
    [NotMapped]
    public static IEnumerable<User> AllUsers {
        get {
          return Context.Users.ToList();
        }
    }

    //[NotMapped]
    //public static IEnumerable<string> AllUsers {
    //    get {
    //        return Participants.Select(p => p.FullName);
    //    }
    //}

    //[NotMapped]
    //public static IEnumerable<User> AllUsers => Context.Users.ToList().Except(ParticipatingUsers());

    //[NotMapped]
    //public static IEnumerable<User> UsersNotParticipating {
    //    get {
    //        return Context.Users.Where(u => !Participants.Contains(u));
    //    }
    //}



    public bool HasVoted(User currentUser) {
        return Choices.Any(c => c.CheckUserVotedOnce(currentUser));
    }

    public int GetTotalVote {
        get{
            return Choices.Sum(choice => choice.Votes.Count);
        }
    }
    public static IQueryable<Poll> GetPolls(User CurrentUser) {
        var allPolls = (Context.Polls
            .Where(poll => poll.Creator.Email.Contains(CurrentUser.Email) ||
                    poll.Participants.Contains(CurrentUser))
            .OrderBy(poll => poll.Name));
       return allPolls;
    }


    public static PollType[] getTypes() {
        return Enum.GetValues(typeof(PollType)).Cast<PollType>().ToArray();

    }
    //public static PollType[] getTypes() =>
    //    Enum.GetValues(typeof(PollType)).Cast<PollType>().ToArray();

    public void Delete() {
        // doit delete les choix, vote, ?
        Choices.Clear();
        Participants.Clear();
        if(commentaires.Count > 0)
            commentaires.Clear();

        Context.Polls.Remove(this);
        Context.SaveChanges();
    }
}

