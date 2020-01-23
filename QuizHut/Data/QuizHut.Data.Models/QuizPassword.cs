namespace QuizHut.Data.Models
{
    using QuizHut.Data.Common.Models;

    public class QuizPassword : BaseDeletableModel<int>
    {
        public string Text { get; set; }

        public int QuizId { get; set; }

        public virtual Quiz Quiz { get; set; }
    }
}
