namespace QuizHut.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using QuizHut.Data.Models;

    public class QuizGroupConfigurartion : IEntityTypeConfiguration<QuizGroup>
    {
        public void Configure(EntityTypeBuilder<QuizGroup> quizGroup)
        {
            quizGroup.HasKey(qg => new { qg.QuizId, qg.GroupId });
        }
    }
}
