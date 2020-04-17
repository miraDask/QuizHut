namespace QuizHut.Web.ViewModels.Answers
{
    using Ganss.XSS;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class AttemtedQuizAnswerViewModel : IMapFrom<Answer>
    {
        public string Id { get; set; }

        public string Text { get; set; }

        public string SanitizedText => new HtmlSanitizer().Sanitize(this.Text);

        public bool IsRightAnswerAssumption { get; set; }
    }
}
