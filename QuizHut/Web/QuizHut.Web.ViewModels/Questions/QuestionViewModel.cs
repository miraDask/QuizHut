namespace QuizHut.Web.ViewModels.Questions
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Answers;
    using QuizHut.Web.ViewModels.Shared;

    public class QuestionViewModel : IMapFrom<Question>
    {
        public string Id { get; set; }

        [Required]
        [StringLength(
           ModelValidations.Question.TextMaxLength,
           ErrorMessage = ModelValidations.Error.RangeMessage,
           MinimumLength = ModelValidations.Question.TextMinLength)]
        public string Text { get; set; }

        public IList<AnswerViewModel> Answers { get; set; } = new List<AnswerViewModel>();

        public int Number { get; set; }

        public string QuizId { get; set; }
    }
}
