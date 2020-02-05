namespace QuizHut.Data.Models
{
    using System;

    using QuizHut.Data.Common.Models;

    public class QuizGroup : BaseDeletableModel<string>
    {
        public QuizGroup()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string QuizId { get; set; }

        public virtual Quiz Quiz { get; set; }

        public string GroupId { get; set; }

        public virtual Group Group { get; set; }
    }
}
