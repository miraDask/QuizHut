namespace QuizHut.Data.Models
{
    using System.Collections.Generic;

    using QuizHut.Data.Common.Models;

    public class Group : BaseDeletableModel<int>
    {
        public Group()
        {
            this.Participants = new HashSet<ApplicationUser>();
        }

        public string Name { get; set; }

        public string CreatorId { get; set; }

        public virtual ApplicationUser Creator { get; set; }

        public virtual ICollection<ApplicationUser> Participants { get; set; }

        public virtual ICollection<QuizGroup> QuizGroups { get; set; }
    }
}
