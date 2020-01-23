namespace QuizHut.Data.Models
{
    using System.Collections.Generic;

    using QuizHut.Data.Common.Models;

    public class Question : BaseDeletableModel<int>
    {
        public Question()
        {
            this.Answers = new HashSet<Answer>();
        }

        public string Text { get; set; }

        public ICollection<Answer> Answers { get; set; }

        public int QuizId { get; set; }

        public virtual Quiz Quiz { get; set; }
    }
}
