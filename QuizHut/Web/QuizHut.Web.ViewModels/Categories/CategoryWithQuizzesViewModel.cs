namespace QuizHut.Web.ViewModels.Categories
{
    using System.Collections.Generic;

    using QuizHut.Web.ViewModels.Quizzes;

    public class CategoryWithQuizzesViewModel
    {
        public CategoryWithQuizzesViewModel()
        {
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public bool Error { get; set; }

        public IList<QuizAssignViewModel> Quizzes { get; set; } = new List<QuizAssignViewModel>();
    }
}
