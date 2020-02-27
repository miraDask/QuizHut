namespace QuizHut.Web.ViewModels.Answers
{
    using System.ComponentModel.DataAnnotations;

    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Shared;

    public class AnswerViewModel : IMapFrom<Answer>
    {
        public string Id { get; set; }

        [Required]
        [MinLength(ModelValidations.Answers.TextMinLength)]
        [MaxLength(ModelValidations.Answers.TextMaxLength)]
        public string Text { get; set; }

        public bool IsRightAnswer { get; set; }

        public string QuestionId { get; set; }
    }
}
