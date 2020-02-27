namespace QuizHut.Web.ViewModels.Quizzes
{
    using System;

    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class QuizListViewModel : IMapFrom<Quiz>, IHaveCustomMappings
    {
        public QuizListViewModel()
        {

        }

        public string Id { get; set; }

        public string Name { get; set; }

        public int QuestionsCount { get; set; }

        public string CreatedOn { get; set; }

        public bool IsActive { get; set; }

        public string Color { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Quiz, QuizListViewModel>()
                .ForMember(
                    x => x.CreatedOn,
                    opt => opt.MapFrom(x => x.CreatedOn.ToString("dd/MM/yyyy")))
                .ForMember(
                    x => x.QuestionsCount,
                    opt => opt.MapFrom(x => x.Questions.Count))
                .ForMember(
                    x => x.IsActive,
                    opt => opt.MapFrom(x => IsTheQuizActive(x.ActivationDateAndTime, x.DurationOfActivity)))
                .ForMember(
                    x => x.Color,
                    opt => opt.MapFrom(x => IsTheQuizActive(x.ActivationDateAndTime, x.DurationOfActivity) == true ? "green" : "red"));
        }

        private static bool IsTheQuizActive(DateTime? activationDateAndTime, TimeSpan? duration)
        {
            if (activationDateAndTime == null)
            {
                return true;
            }

            var now = DateTime.UtcNow;
            if (duration != null)
            {
                var end = activationDateAndTime.Value.Add((TimeSpan)duration);

                if (now < activationDateAndTime || now > end)
                {
                    return false;
                }
            }
            else
            {
                if (now.Date < activationDateAndTime.Value.Date || now.Date > activationDateAndTime.Value.Date)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
