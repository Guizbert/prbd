using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyPoll.Model;

namespace MyPoll.ViewModel;

public class PollsCardViewModel : ViewModelCommon
{
    private readonly Poll _poll;

    public Poll Poll
    {
        get => _poll;
        private init => SetProperty(ref _poll, value);
    }

    public string Name => Poll.Name;
    public PollType Type => Poll.Type;
    public string Creator => Poll.Creator.FullName;
    public int Participants => Poll.Participants.Count;
    public int Votes => Poll.GetTotalVote;
    public IEnumerable<Choice> BestChoice => Poll.BestChoice;

    public PollsCardViewModel(Poll poll) {
        Poll = poll;
    }
}

