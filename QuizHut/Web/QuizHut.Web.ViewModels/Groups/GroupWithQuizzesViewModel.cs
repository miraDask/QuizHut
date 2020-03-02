namespace QuizHut.Web.ViewModels.Groups
{
    using System.Collections.Generic;

    using QuizHut.Web.ViewModels.Quizzes;

    public class GroupWithQuizzesViewModel
    {
        public GroupWithQuizzesViewModel()
        {

        }

        public string GroupId { get; set; }

        public string Name { get; set; }

        public IList<QuizAssignViewModel> Quizzes { get; set; } = new List<QuizAssignViewModel>();
    }
}
