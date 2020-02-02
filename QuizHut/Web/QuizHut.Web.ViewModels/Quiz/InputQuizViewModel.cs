namespace QuizHut.Web.ViewModels.Quiz
{
    using System;
    using System.Collections.Generic;

    using QuizHut.Web.ViewModels.Question;

    public class InputQuizViewModel
    {
        public string Name { get; set; }

        public string CreatorId { get; set; }

        public string Description { get; set; }

        public DateTime? ActivationDate { get; set; }

        public int? Duration { get; set; }

        public IList<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
    }
}
