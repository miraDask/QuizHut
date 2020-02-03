namespace QuizHut.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using QuizHut.Data.Models;

    using static QuizHut.Data.Validations.DataValidation.Category;

    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> category)
        {
            category.HasMany(c => c.Quizzes)
                .WithOne(q => q.Category)
                .HasForeignKey(q => q.CategoryId);

            category.Property(c => c.Name)
                .HasMaxLength(NameMaxLength)
                .IsRequired();
        }
    }
}
