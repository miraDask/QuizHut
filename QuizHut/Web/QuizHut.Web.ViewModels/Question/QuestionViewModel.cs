namespace QuizHut.Web.ViewModels.Question
{
    using System;
    using System.Collections.Generic;

    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Answer;

    public class QuestionViewModel : IMapFrom<Question>
    {
        public QuestionViewModel()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }

        public string Text { get; set; }

        public IList<AnswerViewModel> Answers { get; set; } = new List<AnswerViewModel>();

        public bool IsDeleted { get; set; }

        public int Number { get; set; }

        public string QuizId { get; set; }
    }
}
