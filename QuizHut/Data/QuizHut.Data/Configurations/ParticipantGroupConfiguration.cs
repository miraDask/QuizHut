namespace QuizHut.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using QuizHut.Data.Models;

    public class ParticipantGroupConfiguration : IEntityTypeConfiguration<ParticipantGroup>
    {
        public void Configure(EntityTypeBuilder<ParticipantGroup> participantGroup)
        {
            participantGroup.HasKey(pg => new { pg.ParticipantId, pg.GroupId });
        }
    }
}
