namespace QuizHut.Data.Models
{
    using System;
    using System.Collections.Generic;

    using QuizHut.Data.Common.Models;

    public class Quiz : BaseDeletableModel<string>
    {
        public Quiz()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Questions = new HashSet<Question>();
            this.QuizzesResults = new HashSet<QuizResult>();
            this.Events = new HashSet<Event>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? Timer { get; set; }

        public bool IsActive { get; set; }

        public string CreatorId { get; set; }

        public virtual ApplicationUser Creator { get; set; }

        public string CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public int? PasswordId { get; set; }

        public virtual Password Password { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

        public virtual ICollection<QuizResult> QuizzesResults { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
