namespace QuizHut.Data.Configurations
{
    using Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using static Validations.DataValidation.Question;

    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> question)
        {
            question.HasMany(q => q.Answers)
                .WithOne(q => q.Question)
                .HasForeignKey(q => q.QuestionId);

            question.Property(q => q.Text)
                .HasMaxLength(TextMaxLength)
                .IsRequired();
        }
    }
}
