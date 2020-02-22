namespace QuizHut.Web.ViewModels.Categories
{
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class EditCategoryNameInputViewModel : IMapFrom<Category>
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
