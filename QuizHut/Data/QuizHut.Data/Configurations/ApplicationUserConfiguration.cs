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
               .HasMany(u => u.Claims)
               .WithOne()
               .HasForeignKey(c => c.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            user
                .HasMany(u => u.Logins)
                .WithOne()
                .HasForeignKey(l => l.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            user
                .HasMany(u => u.Roles)
                .WithOne()
                .HasForeignKey(r => r.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            user
                .HasMany(u => u.StudentsInGroups)
                .WithOne(sg => sg.Student)
                .HasForeignKey(sg => sg.StudentId);

            user
                .HasMany(u => u.CreatedQuizzes)
                .WithOne(q => q.Creator)
                .HasForeignKey(q => q.CreatorId);

            user
                .HasMany(u => u.CreatedGroups)
                .WithOne(g => g.Creator)
                .HasForeignKey(g => g.CreatorId);

            user
                .HasMany(u => u.Students)
                .WithOne(u => u.Teacher)
                .HasForeignKey(u => u.TeacherId);

            user
                .HasMany(u => u.Results)
                .WithOne(r => r.Student)
                .HasForeignKey(r => r.StudentId);

            user
                .HasMany(u => u.CreatedEvents)
                .WithOne(e => e.Creator)
                .HasForeignKey(e => e.CreatorId);
        }
    }
}
