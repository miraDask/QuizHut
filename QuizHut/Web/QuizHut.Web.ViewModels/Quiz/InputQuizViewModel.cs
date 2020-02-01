namespace QuizHut.Web.ViewModels.Quiz
{
    using System;
    using System.Collections.Generic;

    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Question;

    public class InputQuizViewModel : IMapTo<Quiz>
    {
        public string Name { get; set; }

        public string CreatorId { get; set; }

        public string Description { get; set; }

        public DateTime? ActivationDate { get; set; }

        public int? Duration { get; set; }

        public IEnumerable<QuestionViewModel> Questions { get; set; }
    }
}
