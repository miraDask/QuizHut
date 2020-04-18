namespace QuizHut.Web.ViewModels.Events
{
    using QuizHut.Data.Common.Enumerations;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class EventSimpleViewModel : IMapFrom<Event>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Status Status { get; set; }
    }
}
