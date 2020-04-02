namespace QuizHut.Web.ViewModels.Questions
{
    using System.Collections.Generic;

    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Answers;

    public class QuestionViewModel : IMapFrom<Question>
    {
        public QuestionViewModel()
        {
            this.Answers = new List<AnswerViewModel>();
        }

        public string Id { get; set; }

        public string Text { get; set; }

        public IList<AnswerViewModel> Answers { get; set; }

        public int Number { get; set; }

        //public string QuizId { get; set; }
    }
}
