namespace QuizHut.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using QuizHut.Data.Models;

    public class EventGroupConfigurartion : IEntityTypeConfiguration<EventGroup>
    {
        public void Configure(EntityTypeBuilder<EventGroup> quizGroup)
        {
            quizGroup.HasKey(qg => new { qg.EventId, qg.GroupId });
        }
    }
}
