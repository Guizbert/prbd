using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPoll.Model;

public class Choix {

    public int ChoixId { get; set; }
    public string Libelle { get; set; }

    public Choix (string libelle) {
        Libelle = libelle;
    }
}
