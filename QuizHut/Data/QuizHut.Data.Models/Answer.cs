namespace QuizHut.Data.Models
{
    using QuizHut.Data.Common.Models;

    public class Answer : BaseDeletableModel<int>
    {
        public string Text { get; set; }

        public bool IsRightAnswer { get; set; }

        public int QuestionId { get; set; }

        public virtual Question Question { get; set; }
    }
}
