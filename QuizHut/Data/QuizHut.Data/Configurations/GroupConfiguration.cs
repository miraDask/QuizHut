namespace QuizHut.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using QuizHut.Data.Models;

    using static Validations.DataValidation.Group;

    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> group)
        {
             group.HasMany(g => g.ParticipanstGroups)
               .WithOne(p => p.Group)
               .HasForeignKey(q => q.GroupId);

             group.HasMany(g => g.QuizzesGroups)
              .WithOne(p => p.Group)
              .HasForeignKey(q => q.GroupId);

             group.Property(g => g.Name)
                .HasMaxLength(NameMaxLength);
        }
    }
}
