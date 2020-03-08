namespace QuizHut.Web.ViewModels.Groups
{
    using System.Collections.Generic;

    public class GroupsListAllViewModel
    {
        public GroupsListAllViewModel()
        {
            this.Groups = new List<GroupListViewModel>();
        }

        public IEnumerable<GroupListViewModel> Groups { get; set; }
    }
}
