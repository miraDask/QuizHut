namespace QuizHut.Web.ViewModels.Groups
{
    using System.Collections.Generic;

    using QuizHut.Web.ViewModels.Quizzes;
    using QuizHut.Web.ViewModels.Students;

    public class GroupDetailsViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<QuizAssignViewModel> Quizzes { get; set; } = new HashSet<QuizAssignViewModel>();

        public IEnumerable<StudentViewModel> Students { get; set; } = new HashSet<StudentViewModel>();
    }
}
