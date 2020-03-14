namespace QuizHut.Web.ViewModels.Quizzes
{
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class QuizResultViewModel : IMapFrom<Result>
    {
        public string QuizName { get; set; }

        public int Points { get; set; }

        public int MaxPoints { get; set; }
    }
}
