namespace QuizHut.Web.ViewModels.Events
{
    using System.Collections.Generic;

    public class ActiveAndEndedEventsViewModel
    {
        public ActiveAndEndedEventsViewModel()
        {
            this.ActiveEvents = new HashSet<SimpleEventViewModel>();
            this.EndedEvents = new HashSet<SimpleEventViewModel>();
        }

        public IEnumerable<SimpleEventViewModel> ActiveEvents { get; set; }

        public IEnumerable<SimpleEventViewModel> EndedEvents { get; set; }
    }
}
