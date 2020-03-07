namespace QuizHut.Web.ViewModels.Groups
{
    using System.Collections.Generic;

    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Events;
    using QuizHut.Web.ViewModels.Students;

    public class GroupDetailsViewModel : IMapFrom<Group>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<EventsAssignViewModel> Events { get; set; } = new HashSet<EventsAssignViewModel>();

        public IEnumerable<StudentViewModel> Students { get; set; } = new HashSet<StudentViewModel>();
    }
}
