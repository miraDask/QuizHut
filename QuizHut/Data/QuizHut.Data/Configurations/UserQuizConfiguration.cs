namespace QuizHut.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Data.Models;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class UserQuizConfiguration : IEntityTypeConfiguration<UserQuiz>
    {
        public void Configure(EntityTypeBuilder<UserQuiz> userQuiz)
        {
            userQuiz.HasKey(u => new { u.UserId, u.QuizId });

            userQuiz.HasOne(uq => uq.User)
                .WithMany(u => u.UsersQuizes)
                .HasForeignKey(uq => uq.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            userQuiz.HasOne(uq => uq.Quiz)
                .WithMany(u => u.UsersQuizes)
                .HasForeignKey(uq => uq.QuizId)
                .OnDelete(DeleteBehavior.Restrict);

            userQuiz.Property(uq => uq.Points)
                .HasDefaultValue(0);

            userQuiz.Property(uq => uq.QuizIsPending)
                .HasDefaultValue(true);
        }
    }
}
