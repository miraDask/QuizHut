namespace QuizHut.Web.ViewModels.Quizzes
{
    using System.Collections.Generic;

    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Questions;

    public class QuizPDFViewModel : IMapFrom<Quiz>
    {
        public QuizPDFViewModel()
        {
            this.Questions = new List<QuestionViewModel>();
        }

        public string Name { get; set; }

        public IEnumerable<QuestionViewModel> Questions { get; set; }
    }
}
