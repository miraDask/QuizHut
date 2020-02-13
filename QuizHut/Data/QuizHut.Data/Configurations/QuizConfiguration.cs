namespace QuizHut.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using QuizHut.Data.Models;

    using static QuizHut.Data.Validations.DataValidation.Quiz;

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
                .HasMaxLength(NameMaxLength)
                .IsRequired();

            quiz.HasIndex(x => x.Password)
                .IsUnique();
        }
    }
}
