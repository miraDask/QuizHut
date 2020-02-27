namespace QuizHut.Web.ViewModels.Categories
{
    using System.ComponentModel.DataAnnotations;

    using QuizHut.Web.ViewModels.Shared;

    public class CreateCategoryInputViewModel
    {
        [Required]
        [MinLength(ModelValidations.Categories.NameMinLength)]
        [MaxLength(ModelValidations.Categories.NameMaxLength)]
        public string Name { get; set; }
    }
}
