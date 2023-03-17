using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;

namespace MyPoll.Model;

public enum Type {
    Simple, Multiple,
}

public class Sondage {
    public int SondageId { get; set; }
    public string Title { get; set; }

    public Type Type { get; set; }
    public User Creator { get; set; }

    public bool IsClosed { get; set; }

    public List<User> Participants { get; set; }

    public List<Vote> Vote { get; set; }

    public List<Choix> choix { get; set; }

    public List<Commentaire> commentaires { get; set; }

    public Sondage(string title, Type type, User creator) {
        Title = title;
        Type = type;
        Creator = creator;
    }

    public Sondage() { }
}

