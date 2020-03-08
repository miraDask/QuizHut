namespace QuizHut.Web.ViewModels.Events
{
    using System.Collections.Generic;

    public class EventsListAllViewModel
    {
        public EventsListAllViewModel()
        {
            this.Events = new HashSet<EventListViewModel>();
        }

        public IEnumerable<EventListViewModel> Events { get; set; }
    }
}
