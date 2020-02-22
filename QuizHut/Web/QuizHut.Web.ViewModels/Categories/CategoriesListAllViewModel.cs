namespace QuizHut.Web.ViewModels.Categories
{
    using System.Collections.Generic;

    public class CategoriesListAllViewModel
    {
        public IList<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    }
}
