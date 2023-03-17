using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPoll.Model;

public class Administrator : User {

    protected Administrator() {
        Role = Role.Administrator;
    }

    public Administrator(string pseudo, string mail, string password)
        : base(pseudo, mail, password) { 
        Role = Role.Administrator;
    }
}

