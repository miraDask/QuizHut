namespace QuizHut.Data
{
    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Models;

    public class QuizHutDbContext : DbContext
    {
        public DbSet<Answer> Answers { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<Quiz> Quizzes { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Code> Codes { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserQuiz> UsersQuizzes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DataSettings.Connection);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }
    }
}
