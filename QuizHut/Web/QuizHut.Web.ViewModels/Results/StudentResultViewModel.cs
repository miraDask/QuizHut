namespace QuizHut.Web.ViewModels.Results
{
    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class StudentResultViewModel : IMapFrom<Result>, IHaveCustomMappings
    {
        public string Event { get; set; }

        public string Quiz { get; set; }

        public string Score { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Result, StudentResultViewModel>()
              .ForMember(
                  x => x.Event,
                  opt => opt.MapFrom(x => x.Event.Name))
              .ForMember(
                  x => x.Quiz,
                  opt => opt.MapFrom(x => x.Event.Quiz.Name))
              .ForMember(
                  x => x.Score,
                  opt => opt.MapFrom(x => $"{x.Points}/{x.MaxPoints}"));
        }
    }
}
