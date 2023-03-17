using System.DirectoryServices.ActiveDirectory;
using PRBD_Framework;

namespace MyPoll.Model;

public enum Role {
    Member = 1,
    Administrator = 2,
}
public class User : EntityBase<MyPollContext> {

    public int UserId { get; set; }
    public string Name { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public Role Role { get; protected set; } = Role.Member;

    public User(string name, string email, string password) {
        Name = name;
        Email = email;
        Password = password;
    }
    public User() { }

}
