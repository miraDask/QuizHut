namespace QuizHut.Web.ViewModels.Events
{
    using System.ComponentModel.DataAnnotations;

    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Shared;

    public class EditEventDetailsInputViewModel : IMapFrom<Event>
    {
        public string Id { get; set; }

        [Required]
        [StringLength(
            ModelValidations.Events.NameMaxLength,
            ErrorMessage = ModelValidations.Error.RangeMessage,
            MinimumLength = ModelValidations.Events.NameMinLength)]
        public string Name { get; set; }
    }
}
