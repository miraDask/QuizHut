namespace QuizHut.Web.ViewModels.Teachers
{
    using System.Collections.Generic;

    using QuizHut.Web.ViewModels.Students;

    public class TeachersAllViewModel
    {
        public TeachersAllViewModel()
        {
            this.Teachers = new HashSet<TeacherViewModel>();
        }

        public UserInputViewModel NewTeacher { get; set; }

        public IEnumerable<TeacherViewModel> Teachers { get; set; }
    }
}
