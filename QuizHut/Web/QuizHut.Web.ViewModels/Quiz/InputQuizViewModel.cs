namespace QuizHut.Web.ViewModels.Quiz
{
    using System.Collections.Generic;

    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class InputQuizViewModel : IMapTo<Quiz>
    {
        public string Name { get; set; }

        public string CreatorId { get; set; }

        public IEnumerable<QuestionViewModel> Questions { get; set; }
    }
}
