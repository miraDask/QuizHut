namespace QuizHut.Web.ViewModels.Question
{
    using System.Collections.Generic;

    using QuizHut.Web.ViewModels.Answer;

    public class QuestionViewModel
    {
        public string Text { get; set; }

        public IList<AnswerViewModel> Answers { get; set; } = new List<AnswerViewModel>();

        public bool IsDeleted { get; set; }
    }
}
