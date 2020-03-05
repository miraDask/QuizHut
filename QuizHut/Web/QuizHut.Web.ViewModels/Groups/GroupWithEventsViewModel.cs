namespace QuizHut.Web.ViewModels.Groups
{
    using System.Collections.Generic;
    using QuizHut.Web.ViewModels.Events;

    public class GroupWithEventsViewModel
    {
        public GroupWithEventsViewModel()
        {

        }

        public string GroupId { get; set; }

        public string Name { get; set; }

        public bool Error { get; set; }

        public IList<EventsAssignViewModel> Events { get; set; } = new List<EventsAssignViewModel>();
    }
}
