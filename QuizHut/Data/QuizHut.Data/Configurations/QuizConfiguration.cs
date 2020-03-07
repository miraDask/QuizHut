namespace QuizHut.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using QuizHut.Data.Models;

    using QuizHut.Data.Validations;

    public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
    {
        public void Configure(EntityTypeBuilder<Quiz> quiz)
        {
            quiz.HasMany(q => q.Questions)
                .WithOne(q => q.Quiz)
                .HasForeignKey(q => q.QuizId);

            quiz.HasMany(q => q.QuizzesResults)
                .WithOne(qr => qr.Quiz)
                .HasForeignKey(qr => qr.QuizId);

            quiz.HasMany(q => q.Events)
              .WithOne(e => e.Quiz)
              .HasForeignKey(e => e.QuizId);

            quiz.HasOne(q => q.Password)
                .WithOne(p => p.Quiz)
                .HasForeignKey<Quiz>(q => q.PasswordId);

            quiz.Property(q => q.Name)
                .HasMaxLength(DataValidation.Quiz.NameMaxLength)
                .IsRequired();
        }
    }
}
