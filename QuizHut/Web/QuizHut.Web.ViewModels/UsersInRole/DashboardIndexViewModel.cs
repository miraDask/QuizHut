namespace QuizHut.Web.ViewModels.UsersInRole
{
    using System.Collections.Generic;

    public class DashboardIndexViewModel
    {
        public DashboardIndexViewModel()
        {
            this.Users = new HashSet<UserInRoleViewModel>();
        }

        public int PagesCount { get; set; }

        public int CurrentPage { get; set; }

        public string SearchType { get; set; }

        public string SearchString { get; set; }

        public UserInputViewModel NewUser { get; set; }

        public IEnumerable<UserInRoleViewModel> Users { get; set; }

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
