namespace QuizHut.Web.ViewModels.Answers
{
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class AttemtedQuizAnswerViewModel : IMapFrom<Answer>
    {
        public AttemtedQuizAnswerViewModel()
        {
        }

        public string Id { get; set; }

        public string Text { get; set; }

        public bool IsRightAnswerAssumption { get; set; }
    }
}
