namespace QuizHut.Data.Configurations
{
    using Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
    {
        public void Configure(EntityTypeBuilder<Quiz> quiz)
        {
            quiz.HasOne(q => q.Category)
                .WithMany(c => c.Quizzes)
                .HasForeignKey(q => q.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            quiz.HasOne(q => q.Creator)
                .WithMany(c => c.CreatedQuizzes)
                .HasForeignKey(q => q.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            quiz.HasOne(q => q.Code)
                .WithOne(c => c.Quiz)
                .HasForeignKey<Quiz>(q => q.CodeId)
                .OnDelete(DeleteBehavior.Restrict);

            quiz.Property(q => q.IsActive)
                .HasDefaultValue(false);

            quiz.Property(q => q.IsStarted)
                .HasDefaultValue(false);
        }
    }
}
