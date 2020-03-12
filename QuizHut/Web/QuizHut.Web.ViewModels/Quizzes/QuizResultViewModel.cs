namespace QuizHut.Web.ViewModels.Quizzes
{
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class QuizResultViewModel : IMapFrom<EventResult>
    {
        public string QuizName { get; set; }

        public int Points { get; set; }

        public int MaxPoints { get; set; }
    }
}
