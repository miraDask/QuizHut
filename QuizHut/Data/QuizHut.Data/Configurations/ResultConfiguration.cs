namespace QuizHut.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using QuizHut.Data.Models;

    public class ResultConfiguration : IEntityTypeConfiguration<Result>
    {
        public void Configure(EntityTypeBuilder<Result> result)
        {
            result.HasMany(r => r.EventsResults)
              .WithOne(er => er.Result)
              .HasForeignKey(er => er.ResultId);
        }
    }
}
