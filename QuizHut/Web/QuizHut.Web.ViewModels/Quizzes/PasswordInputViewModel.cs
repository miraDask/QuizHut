namespace QuizHut.Web.ViewModels.Quizzes
{
    using System.ComponentModel.DataAnnotations;

    using QuizHut.Web.ViewModels.Shared;

    public class PasswordInputViewModel
    {
        [Required]
        [StringLength(
            ModelValidations.Password.PasswordMaxLength,
            ErrorMessage = ModelValidations.Error.RangeMessage,
            MinimumLength = ModelValidations.Password.PasswordMinLength)]
        public string Password { get; set; }

        public string Error { get; set; }
    }
}
