namespace QuizHut.Data.Configurations
{
    using Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using static Validations.DataValidation.Quiz;

    public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
    {
        public void Configure(EntityTypeBuilder<Quiz> quiz)
        {
            quiz.HasMany(q => q.Questions)
                .WithOne(q => q.Quiz)
                .HasForeignKey(q => q.QuizId);

            quiz.HasMany(q => q.QuizResults)
                .WithOne(qr => qr.Quiz)
                .HasForeignKey(qr => qr.QuizId);

            quiz.HasMany(q => q.QuizzesGroups)
               .WithOne(qr => qr.Quiz)
               .HasForeignKey(qr => qr.QuizId);

            quiz.Property(q => q.IsStarted)
                .HasDefaultValue(true);
        }
    }
}
