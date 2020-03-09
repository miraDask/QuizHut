namespace QuizHut.Web.ViewModels.Events
{
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class SimpleEventViewModel : IMapFrom<Event>
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}