namespace QuizHut.Web.ViewModels.Quizzes
{
    using System.Collections.Generic;
    using QuizHut.Data.Models;
    using QuizHut.Web.ViewModels.Answers;
    using QuizHut.Services.Mapping;

    public class AttemtedQuizQuestionViewModel : IMapFrom<Question>
    {
        public AttemtedQuizQuestionViewModel()
        {
        }

        public string Id { get; set; }

        public string Text { get; set; }

        public int Number { get; set; }

        public IList<AttemtedQuizAnswerViewModel> Answers { get; set; } = new List<AttemtedQuizAnswerViewModel>();
    }
}
