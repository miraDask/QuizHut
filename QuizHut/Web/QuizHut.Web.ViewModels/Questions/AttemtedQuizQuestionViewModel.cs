namespace QuizHut.Web.ViewModels.Quizzes
{
    using System.Collections.Generic;

    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Answers;

    public class AttemtedQuizQuestionViewModel : IMapFrom<Question>
    {
        public AttemtedQuizQuestionViewModel()
        {
            this.Answers = new List<AttemtedQuizAnswerViewModel>();
        }

        public string Id { get; set; }

        public string Text { get; set; }

        public int Number { get; set; }

        public IList<AttemtedQuizAnswerViewModel> Answers { get; set; }
    }
}
