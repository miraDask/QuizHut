namespace QuizHut.Data.Models
{
    using System;

    using QuizHut.Data.Common.Models;

    public class QuizResult : BaseDeletableModel<string>
    {
        public QuizResult()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string ParticipantId { get; set; }

        public virtual ApplicationUser Participant { get; set; }

        public string QuizId { get; set; }

        public virtual Quiz Quiz { get; set; }

        public int Points { get; set; }

        public int MaxPoints { get; set; }
    }
}
