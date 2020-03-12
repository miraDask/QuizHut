namespace QuizHut.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using QuizHut.Data.Models;

    public class EventResultConfigurartion : IEntityTypeConfiguration<EventResult>
    {
        public void Configure(EntityTypeBuilder<EventResult> quizResult)
        {
            quizResult.HasKey(qg => new { qg.EventId, qg.ResultId });
        }
    }
}
