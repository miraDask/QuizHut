namespace QuizHut.Web.ViewModels.Events
{
    using System.ComponentModel.DataAnnotations;

    using QuizHut.Web.ViewModels.Shared;

    public class CreateEventInputViewModel
    {
        [Required]
        [StringLength(
         ModelValidations.Events.NameMaxLength,
         ErrorMessage = ModelValidations.Error.RangeMessage,
         MinimumLength = ModelValidations.Events.NameMinLength)]
        public string Name { get; set; }

        [Required]
        [RegularExpression(ModelValidations.RegEx.Date, ErrorMessage = ModelValidations.Error.DateFormatMessage)]
        public string ActivationDate { get; set; }

        [Required]
        [RegularExpression(ModelValidations.RegEx.Time, ErrorMessage = ModelValidations.Error.TimeFormatMessage)]
        public string ActiveFrom { get; set; }

        [Required]
        [RegularExpression(ModelValidations.RegEx.Time, ErrorMessage = ModelValidations.Error.TimeFormatMessage)]
        public string ActiveTo { get; set; }
    }
}
