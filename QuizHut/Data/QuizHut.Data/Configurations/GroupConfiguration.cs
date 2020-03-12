namespace QuizHut.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using QuizHut.Data.Models;

    using QuizHut.Data.Validations;

    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> group)
        {
            group.HasMany(g => g.StudentstGroups)
              .WithOne(p => p.Group)
              .HasForeignKey(q => q.GroupId);

            group.HasMany(g => g.EventsGroups)
             .WithOne(p => p.Group)
             .HasForeignKey(q => q.GroupId);

            group.Property(g => g.Name)
             .HasMaxLength(DataValidation.Group.NameMaxLength)
             .IsRequired();
        }
    }
}
