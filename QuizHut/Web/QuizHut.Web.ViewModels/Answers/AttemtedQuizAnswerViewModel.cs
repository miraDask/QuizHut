namespace QuizHut.Web.ViewModels.Answers
{
    public class AttemtedQuizAnswerViewModel
    {
        public AttemtedQuizAnswerViewModel()
        {
        }

        public string Text { get; set; }

        public bool IsRightAnswer { get; set; }

        public bool IsRightAnswerAssumption { get; set; }
    }
}
