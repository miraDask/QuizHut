namespace QuizHut.Web.ViewModels.Groups
{
    using System.Collections.Generic;

    using QuizHut.Web.ViewModels.Students;

    public class GroupWithStudentsViewModel
    {
        public string GroupId { get; set; }

        public IList<StudentViewModel> Students { get; set; } = new List<StudentViewModel>();
    }
}
