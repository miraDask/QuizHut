namespace QuizHut.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using QuizHut.Data.Models;

    public class CodeConfiguration : IEntityTypeConfiguration<Code>
    {
        public void Configure(EntityTypeBuilder<Code> code)
        {
            code.HasOne(c => c.Quiz)
                .WithOne(q => q.Code)
                .HasForeignKey<Code>(c => c.QuizId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
