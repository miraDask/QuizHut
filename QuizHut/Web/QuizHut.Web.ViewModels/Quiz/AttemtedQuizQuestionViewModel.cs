namespace QuizHut.Web.ViewModels.Quiz
{
    using System.Collections.Generic;

    using QuizHut.Web.ViewModels.Answer;

    public class AttemtedQuizQuestionViewModel
    {
        public string Text { get; set; }

        public int Number { get; set; }

        public IList<AttemtedQuizAnswerViewModel> Answers { get; set; } = new List<AttemtedQuizAnswerViewModel>();
    }
}
