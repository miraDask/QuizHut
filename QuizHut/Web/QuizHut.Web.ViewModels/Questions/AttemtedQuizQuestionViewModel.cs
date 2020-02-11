namespace QuizHut.Web.ViewModels.Quizzes
{
    using System.Collections.Generic;

    using QuizHut.Web.ViewModels.Answers;

    public class AttemtedQuizQuestionViewModel
    {
        public AttemtedQuizQuestionViewModel()
        {
        }

        public string Text { get; set; }

        public int Number { get; set; }

        public IList<AttemtedQuizAnswerViewModel> Answers { get; set; } = new List<AttemtedQuizAnswerViewModel>();
    }
}
