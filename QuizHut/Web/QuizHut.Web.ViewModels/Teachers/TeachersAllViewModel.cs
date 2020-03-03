namespace QuizHut.Web.ViewModels.Teachers
{
    using System.Collections.Generic;

    using QuizHut.Web.ViewModels.Students;

    public class TeachersAllViewModel
    {
        public StudentInputViewModel NewTeacher { get; set; }

        public IEnumerable<TeacherViewModel> Teachers { get; set; } = new HashSet<TeacherViewModel>();
    }
}
