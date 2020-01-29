namespace QuizHut.Web.ViewModels.Quiz
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class QuestionViewModel
    {
        public string Text { get; set; }

        public IEnumerable<AnswerViewModel> Answers { get; set; }
    }
}
