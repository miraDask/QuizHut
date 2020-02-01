namespace QuizHut.Web.ViewModels.Question
{
    using QuizHut.Web.ViewModels.Answer;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class QuestionViewModel
    {
        public string Text { get; set; }

        public int QuizId { get; set; }

        public IEnumerable<AnswerViewModel> Answers { get; set; }
    }
}
