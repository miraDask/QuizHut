namespace QuizHut.Web.ViewModels.Quizzes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Shared;

    public class EditDetailsViewModel : IMapFrom<Quiz>
    {
        public string Id { get; set; }

        [Required]
        [StringLength(
           ModelValidations.Quizzes.NameMaxLength,
           ErrorMessage = ModelValidations.Error.RangeMessage,
           MinimumLength = ModelValidations.Quizzes.NameMinLength)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [StringLength(
           ModelValidations.Quizzes.PasswordMaxLength,
           ErrorMessage = ModelValidations.Error.RangeMessage,
           MinimumLength = ModelValidations.Quizzes.PasswordMinLength)]
        public string Password { get; set; }

        public int? Timer { get; set; }

        public bool PasswordIsValid { get; set; }
    }
}
