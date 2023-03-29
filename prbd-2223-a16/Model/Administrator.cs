

namespace MyPoll.Model;

public class Administrator : User {

    public Administrator() {
        Role = Role.Administrator;
    }

    public Administrator(string pseudo, string mail, string password)
        : base(pseudo, mail, password)
    {
        Role = Role.Administrator;
    }
}

