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

            quiz.HasOne(q => q.Password)
                .WithOne(p => p.Quiz)
                .HasForeignKey<Quiz>(q => q.PasswordId);

            quiz.HasOne(q => q.Event)
                .WithOne(p => p.Quiz)
                .HasForeignKey<Quiz>(q => q.EventId);

            quiz.Property(q => q.Name)
                .HasMaxLength(DataValidation.Quiz.NameMaxLength)
                .IsRequired();
        }
    }
}
