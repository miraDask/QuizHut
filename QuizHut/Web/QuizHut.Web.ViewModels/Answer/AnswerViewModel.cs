namespace QuizHut.Web.ViewModels.Answer
{
    public class AnswerViewModel
    {
        public string Text { get; set; }

        public int QuestionId { get; set; }

        public bool IsRightAnswer { get; set; }
    }
}
