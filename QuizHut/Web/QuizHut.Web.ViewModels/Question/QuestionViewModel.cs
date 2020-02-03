namespace QuizHut.Web.ViewModels.Question
{
    using System;
    using System.Collections.Generic;

    using QuizHut.Web.ViewModels.Answer;

    public class QuestionViewModel
    {
        public QuestionViewModel()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }

        public string Text { get; set; }

        public IList<AnswerViewModel> Answers { get; set; } = new List<AnswerViewModel>();

        public bool IsDeleted { get; set; }

        public int Count { get; set; }
    }
}
