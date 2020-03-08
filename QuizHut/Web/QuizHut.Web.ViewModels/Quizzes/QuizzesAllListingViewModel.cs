namespace QuizHut.Web.ViewModels.Quizzes
{
    using System.Collections.Generic;

    public class QuizzesAllListingViewModel
    {
        public QuizzesAllListingViewModel()
        {
            this.Quizzes = new List<QuizListViewModel>();
        }

        public IEnumerable<QuizListViewModel> Quizzes { get; set; }
    }
}
