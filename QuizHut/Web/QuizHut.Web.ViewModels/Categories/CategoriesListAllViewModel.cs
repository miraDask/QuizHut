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

        public int PagesCount { get; set; }

        public int CurrentPage { get; set; }

        public string SearchType { get; set; }

        public string SearchString { get; set; }

        public int NextPage
        {
            get
            {
                if (this.CurrentPage >= this.PagesCount)
                {
                    return 1;
                }

                return this.CurrentPage + 1;
            }
        }

        public int PreviousPage
        {
            get
            {
                if (this.CurrentPage <= 1)
                {
                    return this.PagesCount;
                }

                return this.CurrentPage - 1;
            }
        }
    }
}
