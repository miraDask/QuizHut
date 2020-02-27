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

            quiz.HasMany(q => q.QuizResults)
                .WithOne(qr => qr.Quiz)
                .HasForeignKey(qr => qr.QuizId);

            quiz.HasMany(q => q.QuizzesGroups)
               .WithOne(qr => qr.Quiz)
               .HasForeignKey(qr => qr.QuizId);

            quiz.Property(q => q.Name)
                .HasMaxLength(DataValidation.Quiz.NameMaxLength)
                .IsRequired();

            quiz.Property(q => q.Password)
               .HasMaxLength(DataValidation.Quiz.PasswordMaxLength)
               .IsRequired();

            quiz.HasIndex(x => x.Password)
                .IsUnique();
        }
    }
}
