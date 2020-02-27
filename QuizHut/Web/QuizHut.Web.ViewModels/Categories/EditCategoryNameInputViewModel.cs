namespace QuizHut.Web.ViewModels.Categories
{
    using System.ComponentModel.DataAnnotations;

    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Shared;

    public class EditCategoryNameInputViewModel : IMapFrom<Category>
    {
        public string Id { get; set; }

        [Required]
        [StringLength(
            ModelValidations.Categories.NameMaxLength,
            ErrorMessage = ModelValidations.Error.RangeMessage,
            MinimumLength = ModelValidations.Categories.NameMinLength)]
        public string Name { get; set; }
    }
}
