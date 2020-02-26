namespace QuizHut.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using QuizHut.Data.Models;

    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> user)
        {
            user
               .HasMany(e => e.Claims)
               .WithOne()
               .HasForeignKey(e => e.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            user
                .HasMany(e => e.Logins)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            user
                .HasMany(e => e.Roles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            user
                .HasMany(e => e.ParticipantInGroups)
                .WithOne(e => e.Participant)
                .HasForeignKey(e => e.ParticipantId);

            user
                .HasMany(e => e.CreatedQuizzes)
                .WithOne(e => e.Creator)
                .HasForeignKey(e => e.CreatorId);

            user
                .HasMany(e => e.CreatedGroups)
                .WithOne(e => e.Creator)
                .HasForeignKey(e => e.CreatorId);

            user
                .HasMany(e => e.Participants)
                .WithOne(e => e.Manager)
                .HasForeignKey(e => e.ManagerId);

            user
                .HasMany(e => e.QuizResults)
                .WithOne(e => e.Participant)
                .HasForeignKey(e => e.ParticipantId);
        }
    }
}
