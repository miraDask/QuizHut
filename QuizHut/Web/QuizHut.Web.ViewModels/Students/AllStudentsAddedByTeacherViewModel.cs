namespace QuizHut.Web.ViewModels.Students
{
    using System.Collections.Generic;

    public class AllStudentsAddedByTeacherViewModel
    {
        public AllStudentsAddedByTeacherViewModel()
        {
            this.Students = new HashSet<StudentViewModel>();
        }

        public string UserId { get; set; }

        public UserInputViewModel NewStudent { get; set; }

        public IEnumerable<StudentViewModel> Students { get; set; }
    }
}
