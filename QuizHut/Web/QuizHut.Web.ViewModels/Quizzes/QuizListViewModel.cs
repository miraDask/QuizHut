namespace QuizHut.Web.ViewModels.Quizzes
{
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

        public string ActivationDate { get; set; }

        public int Duration { get; set; }

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
                    x => x.ActivationDate,
                    opt => opt.MapFrom(x => x.ActivationDateAndTime == null ? string.Empty : x.ActivationDateAndTime.Value.ToString("dd/MM/yyyy")));
        }
    }
}
