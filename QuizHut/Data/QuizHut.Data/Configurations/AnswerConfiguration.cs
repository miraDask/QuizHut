namespace QuizHut.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using QuizHut.Data.Models;

    using static QuizHut.Data.Validations.DataValidation.Answer;

    public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> answer)
        {
            answer.Property(a => a.Text)
                .IsRequired()
                .HasMaxLength(TextMaxLength);

            answer.Property(a => a.IsRightAnswer)
                .HasDefaultValue(false);
        }
    }
}
