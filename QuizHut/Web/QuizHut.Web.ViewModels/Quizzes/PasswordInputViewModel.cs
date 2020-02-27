namespace QuizHut.Web.ViewModels.Quizzes
{
    using System.ComponentModel.DataAnnotations;

    using QuizHut.Web.ViewModels.Shared;

    public class PasswordInputViewModel
    {
        [Required]
        [StringLength(
            ModelValidations.Quizzes.PasswordMaxLength,
            ErrorMessage = ModelValidations.Error.RangeMessage,
            MinimumLength = ModelValidations.Quizzes.PasswordMinLength)]
        public string Password { get; set; }
    }
}
