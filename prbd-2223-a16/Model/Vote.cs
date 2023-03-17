using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPoll.Model;

public enum VoteValue {
    oui = 1, non = -1, peutEtre = 0,
}

public class Vote {

    public User User { get; set; }
    public Choix Choix { get; set; }

    public int Valeur { get; set; }

    public Vote(User user, Choix choix, int valeur) {
        User = user;
        Choix = choix;
        Valeur = valeur;
    }
}
