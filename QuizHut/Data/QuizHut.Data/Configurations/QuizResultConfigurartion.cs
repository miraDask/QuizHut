namespace QuizHut.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using QuizHut.Data.Models;

    public class QuizResultConfigurartion : IEntityTypeConfiguration<QuizResult>
    {
        public void Configure(EntityTypeBuilder<QuizResult> quizResult)
        {
            quizResult.HasKey(qg => new { qg.QuizId, qg.ResultId });
        }
    }
}
