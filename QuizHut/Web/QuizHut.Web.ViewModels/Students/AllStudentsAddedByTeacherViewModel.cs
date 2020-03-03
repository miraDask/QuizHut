namespace QuizHut.Web.ViewModels.Students
{
    using System.Collections.Generic;

    public class AllStudentsAddedByTeacherViewModel
    {
        public string UserId { get; set; }

        public StudentInputViewModel NewStudent { get; set; }

        public IEnumerable<StudentViewModel> Students { get; set; } = new HashSet<StudentViewModel>();
    }
}
