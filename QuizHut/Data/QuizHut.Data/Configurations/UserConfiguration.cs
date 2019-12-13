namespace QuizHut.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Data.Models;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> user)
        {
            user.HasIndex(u => u.Username)
                .IsUnique();

            user.HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
