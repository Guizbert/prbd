﻿using System;
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
    public virtual ICollection<User> Participants { get; set; } = new HashSet<User>();
    public virtual ICollection<Choice> Choices { get; set; } = new HashSet<Choice>();

    public virtual ICollection<Comment> commentaires { get; set; } = new HashSet<Comment>();

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
    public Poll( string name, int creatorId, PollType type,  bool isClosed) {
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
            return choices;
        }
    }

    [NotMapped]
    public double BestChoiceValue {
        get {
            if (Choices.Count == 0) return 0;
            var maxScore = Choices.Select(c => c.Votes.Sum(v => v.Value)).Max();
            return maxScore;
        }
    }
    

    [NotMapped]
    public static IEnumerable<User> AllUsers {
        get {
          return Context.Users.OrderBy(u => u.FullName).ToList();
        }
    }

    public bool HasVoted(User currentUser) {
        return Choices.Any(c => c.CheckUserVotedOnce(currentUser));
    }

    public int GetTotalVote {
        get{
            return Choices.Sum(choice => choice.Votes.Count);
        }
    }

    public static IQueryable<Poll> GetPolls(User CurrentUser) {
        if (CurrentUser.Role == Role.Administrator)
        {
            var allPollsAdmin = (Context.Polls
                .OrderBy(p => p.Name));
            return allPollsAdmin;
        } else
        {
            var allPolls = (Context.Polls
            .Where(poll => poll.Creator.Email.Contains(CurrentUser.Email) ||
                    poll.Participants.Contains(CurrentUser))
            .OrderBy(poll => poll.Name));
            return allPolls;
        }
    }
    public static IQueryable<Poll> GetPollsWhereCreator(User CurrentUser) {
        
        var allPolls = (Context.Polls
        .Where(poll => poll.Creator.Email.Contains(CurrentUser.Email))
        .OrderBy(poll => poll.Name));
        return allPolls;
        
    }
    public static IQueryable<Poll> GetPollsWhereParticipant(User CurrentUser) {
        var allPolls = (Context.Polls
        .Where(poll => poll.Participants.Contains(CurrentUser))
        .OrderBy(poll => poll.Name));
        return allPolls;
    }
    public static PollType[] getTypes() {
        return Enum.GetValues(typeof(PollType)).Cast<PollType>().ToArray();
    }

    public void Delete() {
        // Supprime les choix, les votes, etc.
        Choices.Clear();

        var votesToDelete = Choices.SelectMany(c => c.Votes).ToList();
        Context.Votes.RemoveRange(votesToDelete);

        Choices.Clear();
        Participants.Clear();

        var commentsToDelete = Context.Comments.Where(c => commentaires.Contains(c)).ToList();
        Context.Comments.RemoveRange(commentsToDelete);

        Context.Polls.Remove(this);
        Context.SaveChanges();

       /* // doit delete les choix, vote, ?
        Choices.Clear();
        var v = Choices.SelectMany(c => c.Votes).ToList();
        //v.ForEach(v.Remove(v))
        foreach(Choice c in Choices) {
            c.Votes.Clear();
        }
        Choices.Clear();
        Participants.Clear();
        foreach(var c in commentaires) {
            Context.Comments.Remove(c);
        }
        
        Context.Polls.Remove(this);
        Context.SaveChanges();*/
    }
}
