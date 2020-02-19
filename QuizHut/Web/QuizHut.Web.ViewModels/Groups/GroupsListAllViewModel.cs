namespace QuizHut.Web.ViewModels.Groups
{
    using System.Collections.Generic;

    public class GroupsListAllViewModel
    {
        public IEnumerable<GroupListViewModel> Groups { get; set; } = new HashSet<GroupListViewModel>();
    }
}
