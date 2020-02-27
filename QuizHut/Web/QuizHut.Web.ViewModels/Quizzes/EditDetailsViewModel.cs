namespace QuizHut.Web.ViewModels.Quizzes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Shared;

    public class EditDetailsViewModel : IMapFrom<Quiz>, IHaveCustomMappings
    {
        public string Id { get; set; }

        [Required]
        [StringLength(
           ModelValidations.Quizzes.NameMaxLength,
           ErrorMessage = ModelValidations.Error.RangeMessage,
           MinimumLength = ModelValidations.Quizzes.NameMinLength)]
        public string Name { get; set; }

        public string Description { get; set; }

        [RegularExpression(ModelValidations.RegEx.Date, ErrorMessage = ModelValidations.Error.DateFormatMessage)]
        public string ActivationDate { get; set; }

        [Required]
        [StringLength(
           ModelValidations.Quizzes.PasswordMaxLength,
           ErrorMessage = ModelValidations.Error.RangeMessage,
           MinimumLength = ModelValidations.Quizzes.PasswordMinLength)]
        public string Password { get; set; }

        [RegularExpression(ModelValidations.RegEx.Time, ErrorMessage = ModelValidations.Error.TimeFormatMessage)]
        public string ActiveFrom { get; set; }

        [RegularExpression(ModelValidations.RegEx.Time, ErrorMessage = ModelValidations.Error.TimeFormatMessage)]
        public string ActiveTo { get; set; }

        public int? Timer { get; set; }

        public bool PasswordIsValid { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Quiz, EditDetailsViewModel>()
                .ForMember(
                    x => x.ActivationDate,
                    opt => opt.MapFrom(x => x.ActivationDateAndTime != null
                    ? x.ActivationDateAndTime.Value.ToString("dd/MM/yyyy") : string.Empty))
               .ForMember(
                    x => x.ActiveFrom,
                    opt => opt.MapFrom(
                        x => x.DurationOfActivity != null
                        ? $"{x.ActivationDateAndTime.Value.Hour.ToString("D2")}:{x.ActivationDateAndTime.Value.Minute.ToString("D2")}"
                        : string.Empty))
               .ForMember(
                    x => x.ActiveTo,
                    opt => opt.MapFrom(
                        x => x.DurationOfActivity != null
                        ? $"{x.ActivationDateAndTime.Value.Add((TimeSpan)x.DurationOfActivity).Hour.ToString("D2")}:{x.ActivationDateAndTime.Value.Add((TimeSpan)x.DurationOfActivity).Minute.ToString("D2")}"
                        : string.Empty));
        }
    }
}
