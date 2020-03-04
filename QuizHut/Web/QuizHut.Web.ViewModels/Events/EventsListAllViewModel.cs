namespace QuizHut.Web.ViewModels.Events
{
    using System.Collections.Generic;

    public class EventsListAllViewModel
    {
        public IList<EventListViewModel> Events { get; set; } = new List<EventListViewModel>();
    }
}
