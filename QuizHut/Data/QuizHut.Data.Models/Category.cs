namespace QuizHut.Data.Models
{
    using System.Collections.Generic;

    using QuizHut.Data.Common.Models;

    public class Category : BaseDeletableModel<int>
    {
        public Category()
        {
            this.Quizzes = new HashSet<Quiz>();
        }

        public string Name { get; set; }

        public string CreatorId { get; set; }

        public virtual ApplicationUser Creator { get; set; }

        public virtual ICollection<Quiz> Quizzes { get; set; }
    }
}
