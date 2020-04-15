namespace QuizHut.Web.ViewModels.Events
{
    using System.Collections.Generic;

    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Quizzes;

    public class EventWithQuizzesViewModel : IMapFrom<Event>
    {
        public EventWithQuizzesViewModel()
        {
            this.Quizzes = new List<QuizAssignViewModel>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public bool Error { get; set; }

        public string TimeZone { get; set; }

        public IList<QuizAssignViewModel> Quizzes { get; set; }
    }
}
