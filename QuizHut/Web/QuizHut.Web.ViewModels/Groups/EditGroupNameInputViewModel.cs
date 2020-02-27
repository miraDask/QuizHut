namespace QuizHut.Web.ViewModels.Groups
{
    using System.ComponentModel.DataAnnotations;

    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Shared;

    public class EditGroupNameInputViewModel : IMapFrom<Group>
    {
        public string Id { get; set; }

        [Required]
        [MinLength(ModelValidations.Groups.NameMinLength)]
        [MaxLength(ModelValidations.Groups.NameMaxLength)]
        public string Name { get; set; }
    }
}
