namespace QuizHut.Web.ViewModels.Events
{
    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Results;

    public class StudentEndedEventViewModel : IMapFrom<Event>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string QuizName { get; set; }

        public string Date { get; set; }

        public ScoreViewModel Score { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Event, StudentEndedEventViewModel>()
                 .ForMember(
                    x => x.Date,
                    opt => opt.MapFrom(x => x.ActivationDateAndTime.Date.ToString("dd/MM/yyyy")));
        }
    }
}
