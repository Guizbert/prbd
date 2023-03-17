using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPoll.Model;
public class Commentaire {

    public string Text { get; set; }
    public DateTime Date{ get; set; }

    public Commentaire(string text) {
        Text = text;
        Date = DateTime.Now;
    }

    public Commentaire() { }
}
