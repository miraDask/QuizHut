namespace QuizHut.Web.ViewModels.Answers
{
    using System;

    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class AnswerViewModel : IMapFrom<Answer>
    {
        public string Id { get; set; }

        public string Text { get; set; }

        public bool IsRightAnswer { get; set; }

        public bool IsDeleted { get; set; } //???

        public int Count { get; set; }

        public string QuestionId { get; set; }
    }
}
