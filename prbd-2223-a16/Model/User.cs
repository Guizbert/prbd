﻿using System.DirectoryServices.ActiveDirectory;
using PRBD_Framework;

namespace MyPoll.Model;

public enum Role {
    Member = 1,
    Administrator = 2,
}
public class User : EntityBase<MyPollContext> {

    public int Id { get; set; }
    public string FullName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public Role Role { get; protected set; } = Role.Member;

    public virtual ICollection<Vote> Votes{ get; set; } = new HashSet<Vote>();
    public virtual ICollection<Poll> UserPolls{ get; set; } = new HashSet<Poll>();
    public virtual ICollection<Comment> Comments{ get; set; } = new HashSet<Comment>();

    public User(string fullName, string email, string password) {
        FullName = fullName;
        Email = email;
        Password = password;
    }
    public User() { }

    /**
     *  TODO :
     *  
     *      -> creer un commentaire ;
     *      -> faire un vote ;
     *      -> faire un choix ;
     *      -> créer sondage ;
     */

}
