namespace QuizHut.Web.ViewModels.Quizzes
{
    using System.Collections.Generic;

    public class QuizzesAllListingViewModel
    {
        public IList<QuizListViewModel> Quizzes { get; set; } = new List<QuizListViewModel>();
    }
}
