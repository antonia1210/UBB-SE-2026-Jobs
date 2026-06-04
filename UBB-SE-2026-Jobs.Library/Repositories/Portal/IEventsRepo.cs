namespace UBB_SE_2026_Jobs.Library.Repositories.Portal
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using UBB_SE_2026_Jobs.Library.Domain.Portal;

    public interface IEventsRepo
    {
        void AddEventToRepo(Event e);
        void RemoveEventFromRepo(Event e);
        ObservableCollection<Event> GetCurrentEventsFromRepo(int? loggedInUser=null);
        ObservableCollection<Event> GetPastEventsFromRepo(int? loggedInUser=null);
        void UpdateEventToRepo(int id, string photo, string title, string description, DateTime start, DateTime end, string location);
    }
}
