using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;

namespace MyPoll.ViewModel;
public class PollsViewModel : ViewModelCommon
{
    private ObservableCollection<PollsCardViewModel> _polls; 
}

