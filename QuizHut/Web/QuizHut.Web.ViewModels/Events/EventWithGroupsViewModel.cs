namespace QuizHut.Web.ViewModels.Events
{
    using System.Collections.Generic;

    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Groups;

    public class EventWithGroupsViewModel : IMapFrom<Event>
    {
        public EventWithGroupsViewModel()
        {
            this.Groups = new List<GroupAssignViewModel>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public bool Error { get; set; }

        public IList<GroupAssignViewModel> Groups { get; set; }
    }
}
