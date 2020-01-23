namespace QuizHut.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using QuizHut.Data.Models;

    public class QuizPasswordConfiguration : IEntityTypeConfiguration<QuizPassword>
    {
        public void Configure(EntityTypeBuilder<QuizPassword> code)
        {
            code.HasOne(c => c.Quiz)
                .WithOne(q => q.QuizPassword)
                .HasForeignKey<QuizPassword>(c => c.QuizId);
        }
    }
}
