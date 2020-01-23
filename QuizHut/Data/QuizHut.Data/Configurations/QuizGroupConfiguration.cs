namespace QuizHut.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using QuizHut.Data.Models;
    using static Validations.DataValidation.QuizGroup;

    public class QuizGroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> group)
        {
             // group.HasMany(g => g.Participants)
             //   .WithOne(p => p.QuizGroup)
             //   .HasForeignKey(q => q.QuizGroupId);
        }
    }
}
