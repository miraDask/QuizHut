namespace QuizHut.Web.ViewModels.Categories
{
    using System.Collections.Generic;

    public class CategoriesListAllViewModel
    {
        public CategoriesListAllViewModel()
        {
            this.Categories = new HashSet<CategoryViewModel>();
        }

        public IEnumerable<CategoryViewModel> Categories { get; set; }
    }
}
