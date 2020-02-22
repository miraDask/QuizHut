namespace QuizHut.Web.ViewModels.Answers
{
    using System.ComponentModel.DataAnnotations;

    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class AnswerViewModel : IMapFrom<Answer>
    {
        public string Id { get; set; }

        [Required]
        public string Text { get; set; }

        public bool IsRightAnswer { get; set; }

        public string QuestionId { get; set; }
    }
}
