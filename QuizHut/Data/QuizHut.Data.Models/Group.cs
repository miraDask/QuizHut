namespace QuizHut.Data.Models
{
    using System.Collections.Generic;

    using QuizHut.Data.Common.Models;

    public class Group : BaseDeletableModel<int>
    {
        public Group()
        {
            this.ParticipanstGroups = new HashSet<ParticipantGroup>();

            this.QuizzesGroups = new HashSet<QuizGroup>();
        }

        public string Name { get; set; }

        public string CreatorId { get; set; }

        public virtual ApplicationUser Creator { get; set; }

        public virtual ICollection<ParticipantGroup> ParticipanstGroups { get; set; }

        public virtual ICollection<QuizGroup> QuizzesGroups { get; set; }
    }
}
