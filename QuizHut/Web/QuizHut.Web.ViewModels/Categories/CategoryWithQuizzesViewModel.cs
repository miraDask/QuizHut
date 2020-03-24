namespace QuizHut.Web.ViewModels.Categories
{
    using System.Collections.Generic;

    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Quizzes;

    public class CategoryWithQuizzesViewModel : IMapFrom<Category>
    {
        public CategoryWithQuizzesViewModel()
        {
            this.Quizzes = new List<QuizAssignViewModel>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public bool Error { get; set; }

        public IList<QuizAssignViewModel> Quizzes { get; set; }
    }
}
