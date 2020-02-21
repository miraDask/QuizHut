namespace QuizHut.Web.ViewModels.Groups
{
    using System.Collections.Generic;

    public class GroupsListAllViewModel
    {
        public IList<GroupListViewModel> Groups { get; set; } = new List<GroupListViewModel>();
    }
}
