using System.ComponentModel.DataAnnotations.Schema;
using System.DirectoryServices.ActiveDirectory;
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
    //collection des polls du user
    public virtual ICollection<Poll> UserPolls{ get; set; } = new HashSet<Poll>();

    [InverseProperty(nameof(Poll.Creator))] 
    public virtual ICollection<Poll> participatingIn{ get; set; } = new HashSet<Poll>();
    //2e iColl de poll pour les participations;
    public virtual ICollection<Comment> Comments{ get; set; } = new HashSet<Comment>();

    public User(string fullName, string email, string password) {
        FullName = fullName;
        Email = email;
        Password = password;
    }
    public User() { }


    public bool isAdmin() { return this.Role == Role.Administrator; }

    /**
     *  TODO :
     *  
     *      -> creer un commentaire ;
     *      -> faire un vote ;
     *      -> faire un choix ;
     *      -> créer sondage ;
     */


    public static User GetByEmail(string mail) {
        return Context.Users.SingleOrDefault(u => u.Email== mail);
    }

}
